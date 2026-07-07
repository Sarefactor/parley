using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Providers;
using Parley.Workflows.Nodes.Factories;

namespace Parley.Workflows.Examples;

public class TestWorkflowFactory
{
    private readonly IAgentSchemaProvider _agentSchemaProvider;
    private readonly IWorkflowSchemaProvider _workflowSchemaProvider;
    private readonly IParleyNodeFactory _parleyNodeFactory;

    public TestWorkflowFactory(IAgentSchemaProvider agentSchemaProvider,
                               IWorkflowSchemaProvider workflowSchemaProvider,
                               IParleyNodeFactory parleyNodeFactory)
    {
        _agentSchemaProvider = agentSchemaProvider;
        _workflowSchemaProvider = workflowSchemaProvider;
        _parleyNodeFactory = parleyNodeFactory;
    }

    public IEnumerable<(Workflow workflow, WorkflowSchema workflowSchema)> BuildWorkflowsFromSchema()
    {
        AgentSchema agentSchema = _agentSchemaProvider.Provide().Result;

        foreach (var workflowSchema in agentSchema.WorkflowSchemas)
        {
            var nodes = workflowSchema.Nodes.ToDictionary(x => x.Key,
                                                          x => _parleyNodeFactory.CreateNode(x.Value, workflowSchema));

            var workflow = new WorkflowBuilder(nodes.First(x => x.Key == workflowSchema.ExecutionNodeId).Value);

            foreach (var node in nodes)
            {
                workflow = node.Value.Configure(workflow, nodes);
            }

            yield return (workflow.Build(), workflowSchema);
        }
    }

    public async Task<(Workflow workflow, WorkflowSchema workflowSchema)> BuildWorkflowFromSchema(Guid workflowId)
    {
        WorkflowSchema workflowSchema = await _workflowSchemaProvider.Provide(workflowId);

        var nodes = workflowSchema.Nodes.ToDictionary(x => x.Key,
                                                           x => _parleyNodeFactory.CreateNode(x.Value, workflowSchema));

        var workflow = new WorkflowBuilder(nodes.First(x => x.Key == workflowSchema.ExecutionNodeId).Value);

        foreach (var node in nodes)
        {
            workflow = node.Value.Configure(workflow, nodes);
        }

        return (workflow.Build(), workflowSchema);
    }
}