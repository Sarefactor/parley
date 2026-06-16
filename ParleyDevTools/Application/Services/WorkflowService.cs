using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Providers;
using Parley.Workflows.Examples;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.Nodes.Factories;

namespace ParleyDevTools.Application.Services;

public class WorkflowService : IWorkflowService
{
    private readonly ISchemaProvider _schemaProvider;
    private readonly IParleyNodeFactory _parleyNodeFactory;

    public WorkflowService(ISchemaProvider schemaProvider,
                           IParleyNodeFactory parleyNodeFactory)
    {
        _schemaProvider = schemaProvider;
        _parleyNodeFactory = parleyNodeFactory;
    }

    public async Task RunParleyWorkflowsAsync()
    {
        try
        {
            var factory = new TestWorkflowFactory(_schemaProvider,
                                                 _parleyNodeFactory);

            foreach (var (workflow, workflowSchema) in factory.BuildWorkflowsFromSchema())
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
}