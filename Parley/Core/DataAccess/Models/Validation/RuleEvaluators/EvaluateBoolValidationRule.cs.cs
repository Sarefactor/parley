using Parley.Core.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Core.DataAccess.Models.Validation.RuleEvaluators;

public static class BoolValidationRuleValidator
{
    public static void ValidateBoolTransitionRule(Guid workflowId,
                                                  Guid nodeId,
                                                  Guid targetNodeId,
                                                  string variableName,
                                                  ValidationRuleDto dto,
                                                  ParleyValidationContext context)
    {
        var initialMessage = $"The transition targeting node: {targetNodeId} evaluates against the date time variable: {variableName}.";

        Evaluate(workflowId,
                 nodeId,
                 dto,
                 initialMessage,
                 context);
    }

    public static void ValidateBoolValidationRule(Guid workflowId,
                                                  Guid nodeId,
                                                  string variableName,
                                                  ValidationRuleDto dto,
                                                  ParleyValidationContext context)
    {
        var initialMessage = $"Invalid validation rule on node: {nodeId} evaluating {variableName}.";

        Evaluate(workflowId,
                 nodeId,
                 dto,
                 initialMessage,
                 context);
    }

    private static void Evaluate(Guid workflowId,
                                 Guid nodeId,
                                 ValidationRuleDto dto,
                                 string initialMessage,
                                 ParleyValidationContext context)
    {
        if (!Enum.IsDefined(dto.BoolComparisonType)
            || dto.BoolComparisonType == BoolComparisonType.None)
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} The {nameof(BoolComparisonType)} is either invalid or has not been set.",
                                 WorkflowErrorType.Schema,
                                 true);

            return;
        }

        if ((dto.BoolComparisonType == BoolComparisonType.EqualTo
            || dto.BoolComparisonType == BoolComparisonType.NotEqualTo)
            && dto.MatchBool == null)
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} The comparison type is {dto.BoolComparisonType} but the value for {nameof(TransitionRule.MatchBool)} has not been set.",
                                 WorkflowErrorType.Config,
                                 true);

            return;
        }
    }
}