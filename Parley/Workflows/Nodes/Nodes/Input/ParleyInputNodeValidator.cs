using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Workflows.Links;
using Parley.Workflows.Nodes.Events;
using Parley.Workflows.State;
using Parley.Workflows.Validation;

namespace Parley.Workflows.Nodes.Nodes.Input;

[SendsMessage(typeof(ParleyLink))]
[SendsMessage(typeof(ParleyInputLink))]
internal sealed class ParleyInputNodeValidator : Executor<string>
{
    private readonly IValidateInput _inputValidator;
    private readonly IWorkflowStateManager _workflowStateManager;

    public ParleyInputNodeValidator(string id,
                                    NodeConfig nodeConfig,
                                    InputNodeOptions options,
                                    IWorkflowStateManager workflowStateManager,
                                    IValidateInput inputValidator)
        : base(id)
    {
        NodeConfig = nodeConfig;
        NodeOptions = options;
        _workflowStateManager = workflowStateManager;
        _inputValidator = inputValidator;
    }

    private NodeConfig NodeConfig { get; set; }
    private InputNodeOptions NodeOptions { get; set; } = new();

    public override async ValueTask HandleAsync(string message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        var workflowVariable = await _workflowStateManager.GetWorkflowVariable(context,
                                                                               NodeOptions.TargetKey,
                                                                               cancellationToken);      

        if(_inputValidator.Validate(workflowVariable, message, NodeConfig.ValidationRules))
        {
            workflowVariable.SetValue(message);
            await _workflowStateManager.SetWorkflowVariable(context, workflowVariable, cancellationToken);

            await context.AddEventAsync(new ParleyMessageEvent("That do be correct."), cancellationToken);
            await context.SendMessageAsync(new ParleyLink(NodeConfig.PrimaryTransitionNode), cancellationToken);
        }
        else
        {
            await context.AddEventAsync(new ParleyMessageEvent(NodeOptions.ErrorMessage), cancellationToken);
            await context.SendMessageAsync(new ParleyInputLink { Message = NodeOptions.Message }, cancellationToken);
        }
    }
}
