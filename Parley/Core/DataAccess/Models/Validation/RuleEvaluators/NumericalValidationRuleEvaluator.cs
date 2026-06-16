using Parley.Core.DataAccess.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Core.DataAccess.Models.Validation.RuleEvaluators;

public static class NumericalValidationRuleEvaluator
{
    public static void ValidateNumericalTransitionRule(Guid workflowId,
                                                       Guid nodeId,
                                                       Guid targetNodeId,
                                                       string variableName,
                                                       ValidationRuleDto dto,
                                                       ParleyValidationContext context)
    {
        var initialMessage = $"The transition targeting node: {targetNodeId} evaluates against the numerical variable: {variableName}.";

        Evaluate(workflowId,
                 nodeId,
                 dto,
                 initialMessage,
                 context);
    }

    public static void ValidateNumericalValidationRule(Guid workflowId,
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
        if (!Enum.IsDefined(dto.NumberComparisonType)
            || dto.NumberComparisonType == NumberComparisonType.None)
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} The {nameof(NumberComparisonType)} is either invalid or has not been set.",
                                 WorkflowErrorType.Schema,
                                 true);

            return;
        }

        if ((dto.NumberComparisonType == NumberComparisonType.EqualTo
            || dto.NumberComparisonType == NumberComparisonType.NotEqualTo
            || dto.NumberComparisonType == NumberComparisonType.GreaterThan
            || dto.NumberComparisonType == NumberComparisonType.GreaterThanOrEqualTo
            || dto.NumberComparisonType == NumberComparisonType.LessThan
            || dto.NumberComparisonType == NumberComparisonType.LessThanOrEqualTo)
            && dto.MatchInt == null)
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} A value for {nameof(TransitionRule.MatchInt)} has not been set.",
                                 WorkflowErrorType.Schema,
                                 true);
        }
    }
}
