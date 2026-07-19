using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Core.Services;
using Parley.Providers;
using Parley.Workflows.Examples;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.Nodes.Factories;

namespace ParleyDevTools.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IAgentSchemaProvider _agentSchemaProvider;
    private readonly IWorkflowSchemaProvider _workflowSchemaProvider;
    private readonly IParleyNodeFactory _parleyNodeFactory;
    private readonly IWorkflowSchemaRegistry _workflowSchemaRegistry;

    private const int PageSize = 10;

    public WorkflowService(IAgentSchemaProvider schemaProvider,
                           IWorkflowSchemaProvider workflowSchemaProvider,
                           IParleyNodeFactory parleyNodeFactory,
                           IWorkflowSchemaRegistry workflowSchemaRegistry)
    {
        _agentSchemaProvider = schemaProvider;
        _workflowSchemaProvider = workflowSchemaProvider;
        _parleyNodeFactory = parleyNodeFactory;
        _workflowSchemaRegistry = workflowSchemaRegistry;
    }
 
    public async Task RunParleyWorkflowsAsync()
    {
        try
        {
            var workflowId = await SelectWorkflowAsync();

            if (workflowId == null)
                return;

            var factory = new TestWorkflowFactory(_agentSchemaProvider,
                                                  _workflowSchemaProvider,
                                                  _parleyNodeFactory);

            var (workflow, workflowSchema) = await factory.BuildWorkflowFromSchema((Guid)workflowId);

            await RunWorkflow(workflow, workflowSchema);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    private static async Task RunWorkflow(Workflow workflow, WorkflowSchema workflowSchema)
    {
        Console.WriteLine($"Workflow: {workflowSchema.Name}");
        Console.Write("Workflow Input: ");

        var workflowInput = Console.ReadLine();

        await using StreamingRun handle = await InProcessExecution.RunStreamingAsync(workflow, new ParleyLink(workflowSchema.ExecutionNodeId) { LinkMessage = workflowInput });

        await foreach (WorkflowEvent evt in handle.WatchStreamAsync())
        {
            //Console.WriteLine(evt.GetType().ToString());

            switch (evt)
            {
                case RequestInfoEvent requestInputEvt:
                    ExternalResponse response = HandleExternalRequest(requestInputEvt.Request);
                    await handle.SendResponseAsync(response);
                    break;

                case WorkflowOutputEvent outputEvt:
                    Console.WriteLine($"Workflow completed with result: {outputEvt.Data}");
                    return;

                case ParleyMessageEvent:
                    HandleParleyMessageEvent(evt);
                    break;
            }
        }
    }

    private static ExternalResponse HandleExternalRequest(ExternalRequest request)
    {
        if (request.TryGetDataAs<ParleyInputLink>(out var parleyLink))
        {
            string input = string.Empty;

            if (parleyLink.Type == ParleyInputType.Plain)
            {
                input = ReadFromConsole($"{parleyLink.Message} : ");
            }

            if (parleyLink.Type == ParleyInputType.Choice)
            {
                Console.WriteLine($"{parleyLink.Message}");
                Console.WriteLine("Select from the following choices:");

                foreach (var choice in parleyLink.Choices)
                {
                    Console.WriteLine(choice);
                }

                input = ReadFromConsole($"Your selection: ");
            }

            return request.CreateResponse(input);
        }

        throw new NotSupportedException($"Request {request.PortInfo.RequestType} is not supported");
    }

    private static string ReadFromConsole(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            Console.WriteLine("Invalid input.");
        }
    }

    private static void HandleParleyMessageEvent(WorkflowEvent workflowEvent)
    {
        if (workflowEvent is ParleyMessageEvent messageEvent)
            Console.WriteLine(messageEvent.Message);
    }

    private async Task<Guid?> SelectWorkflowAsync()
    {
        const int previousOption = PageSize + 1;
        const int nextOption = PageSize + 2;
        const int exitOption = PageSize + 3;

        var skip = 0;

        while (true)
        {
            Console.WriteLine("Loading workflows...");
            var result = await _workflowSchemaRegistry.Search(skip, PageSize);

            if (result.TotalResults == 0)
            {
                Console.WriteLine("No workflows found, press any key to continue...");
                Console.ReadKey(true);
                Console.WriteLine();
                return null;
            }

            if (result.Results.Count == 0 && skip > 0)
            {
                skip = Math.Max(0, skip - PageSize);
                continue;
            }

            Console.WriteLine($"Workflow Search Results ({skip + 1} to {skip + result.Results.Count} of {result.TotalResults}):\n");

            for (var i = 0; i < result.Results.Count; i++)
                Console.WriteLine($"{i + 1}) {result.Results[i].Name}");

            var hasPrevious = skip > 0;
            var hasNext = skip + result.Results.Count < result.TotalResults;

            Console.WriteLine("\nPagination Options");
            if (hasPrevious) Console.WriteLine($"{previousOption}) Previous Search Results.");
            if (hasNext) Console.WriteLine($"{nextOption}) Next Search Results.");
            Console.WriteLine($"{exitOption}) Select To Exit.\n");

            while (true)
            {
                Console.Write("Select an Option: ");

                if (!int.TryParse(Console.ReadLine(), out var option))
                    continue;

                if (option >= 1 && option <= result.Results.Count)
                {
                    Console.WriteLine();
                    return result.Results[option - 1].Id;
                }

                if (option == previousOption && hasPrevious) { skip -= PageSize; break; }
                if (option == nextOption && hasNext) { skip += PageSize; break; }
                if (option == exitOption) return null;
            }
        }
    }
}