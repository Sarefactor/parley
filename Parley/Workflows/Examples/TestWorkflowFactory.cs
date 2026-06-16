using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Providers;
using Parley.Workflows.Nodes.Factories;

namespace Parley.Workflows.Examples;

public class TestWorkflowFactory
{
    private readonly ISchemaProvider _schemaProvider;
    private readonly IParleyNodeFactory _parleyNodeFactory;

    public TestWorkflowFactory(ISchemaProvider schemaProvider,
                               IParleyNodeFactory parleyNodeFactory)
    {
        _schemaProvider = schemaProvider;
        _parleyNodeFactory = parleyNodeFactory;
    }

    public IEnumerable<(Workflow workflow, WorkflowSchema workflowSchema)> BuildWorkflowsFromSchema()
    {
        AgentSchema agentSchema = _schemaProvider.Provide().Result;

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
}