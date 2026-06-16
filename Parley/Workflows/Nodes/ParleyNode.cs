using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Workflows.Links;
using Parley.Workflows.State;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Parley.Workflows.Nodes;

public abstract class ParleyNode<TInput> : Executor<TInput>
{
    protected IWorkflowStateManager WorkflowStateManager { get; set; }
    private static readonly JsonSerializerOptions OptionsJson = new(JsonSerializerDefaults.Web);

    public ParleyNode(string parentNodeName,
                      ParleyNodeContext context,
                      IWorkflowStateManager workflowStateManager)
        : base($"{parentNodeName}:{context.NodeConfig.NodeId}")
    {
        NodeId = context.NodeConfig.NodeId;
        NodeConfig = context.NodeConfig;
        WorkflowStateManager = workflowStateManager;
    }

    public Guid NodeId { get; protected set; }

    public abstract string DialogType { get; }

    public NodeConfig NodeConfig { get; set; }

    protected TResult GetNodeOptions<TResult>() where TResult : ParleyNodeOptions
    {
        return NodeConfig.Options.Deserialize<TResult>(OptionsJson)
            ?? throw new InvalidOperationException($"Options for node {NodeConfig.NodeId} could not be read as {typeof(TResult).Name}.");
    }

    public virtual WorkflowBuilder Configure(WorkflowBuilder builder,
                                             Dictionary<Guid, ParleyNode<ParleyLink>> nodes)
    {
        var transitionNode = nodes.Single(x => x.Key == NodeConfig.PrimaryTransitionNode).Value;
        builder.AddEdge(this, transitionNode);

        return builder;
    }

    protected string BuildMessage(ICollection<WorkflowVariable> variables, string baseMessage)
    {
        return Regex.Replace(baseMessage, @"\[([^\]]*)\]", match =>
        {
            var variable = variables.FirstOrDefault(x => x.Name == match.Groups[1].Value);
            return variable?.Value?.ToString() ?? match.Value;
        });
    }

    protected async Task<string> BuildMessage(IWorkflowContext context, string baseMessage, CancellationToken cancellationToken)
    {
        var variables = await WorkflowStateManager.GetWorkflowVariablesFromContext(context, cancellationToken);
        return BuildMessage(variables, baseMessage);
    }
}

public class ParleyNodeOptions();