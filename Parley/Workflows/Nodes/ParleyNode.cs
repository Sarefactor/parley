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

    protected async Task<string> BuildMessage(IWorkflowContext context, ICollection<WorkflowVariable> variables, string baseMessage, CancellationToken cancellationToken)
    {
        var regex = new Regex(@"\[([^\]]*)\]");
        var replacements = new Dictionary<string, string>();

        foreach (Match match in regex.Matches(baseMessage))
        {
            if (replacements.ContainsKey(match.Value))
                continue;

            replacements[match.Value] = await ResolveVariableAsync(
                context, variables, match.Groups[1].Value, cancellationToken);
        }

        return replacements.Count() > 0 ? regex.Replace(baseMessage, match => replacements[match.Value])
                                        : baseMessage;
    }

    private async Task<string> ResolveVariableAsync(IWorkflowContext context,
                                                    ICollection<WorkflowVariable> variables,
                                                    string targetKey,
                                                    CancellationToken cancellationToken)
    {
        var variableKey = ParleyVariable.ParseKey(targetKey);
        var variable = variables.FirstOrDefault(x => x.Name == variableKey);

        if (variable == null)
            return targetKey;

        var variableContext = variable.BuildVariableContext(targetKey);
        await SetVariableIterationContext(variableContext, context, cancellationToken);

        return variable.GetVariableValueAsString(targetKey, variableContext) ?? targetKey;
    }

    protected async Task SetVariableIterationContext(VariableIterationContext variableIterationContext,
                                                     IWorkflowContext workflowContext,
                                                     CancellationToken cancellationToken)
    {
        if (!variableIterationContext.HasList())
            return;

        (var primaryContext, var secondaryContext) = await WorkflowStateManager.GetWorkflowVariableContexts(variableIterationContext.PrimaryContext.Key,
                                                                                                            variableIterationContext.SecondaryContext?.Key,
                                                                                                            workflowContext,
                                                                                                            cancellationToken);

        if (primaryContext != null)
            variableIterationContext.SetIterationContext(primaryContext, true);

        if (secondaryContext != null)
            variableIterationContext.SetIterationContext(secondaryContext, false);

        return;
    }

    protected async Task<string> BuildMessage(IWorkflowContext context, string baseMessage, CancellationToken cancellationToken)
    {
        var variables = await WorkflowStateManager.GetWorkflowVariablesFromContext(context, cancellationToken);
        return await BuildMessage(context, variables, baseMessage, cancellationToken);
    }

    protected async Task SetWorkflowVariable(IWorkflowContext context,
                                         string variableKey,
                                         object value,
                                         CancellationToken cancellationToken)
    {
        var workflowVariable = await WorkflowStateManager.GetWorkflowVariable(context,
                                                                              variableKey,
                                                                              cancellationToken);

        workflowVariable.SetValue(value);

        await WorkflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);
    }
}

public class ParleyNodeOptions();