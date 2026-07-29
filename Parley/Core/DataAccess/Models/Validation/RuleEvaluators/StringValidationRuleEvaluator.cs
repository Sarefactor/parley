using Parley.Core.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;
using System.Text.RegularExpressions;

namespace Parley.Core.DataAccess.Models.Validation.RuleEvaluators;

public static class StringValidationRuleEvaluator
{
    public static void ValidateForTransition(Guid workflowId,
                                             Guid nodeId,
                                             Guid targetNodeId,
                                             string variableName,
                                             ValidationRuleDto dto,
                                             ParleyValidationContext context)
    {
        var initialMessage = $"The transition targeting node: {targetNodeId} evaluates against the string variable: {variableName}.";

        Evaluate(workflowId,
                 nodeId,
                 dto,
                 initialMessage,
                 context);
    }

    public static void Validate(Guid workflowId,
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


        if (!Enum.IsDefined(dto.StringComparisonType)
            || dto.StringComparisonType == StringComparisonType.None)
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} The {nameof(StringComparisonType)} is either invalid or has not been set.",
                                 WorkflowErrorType.Schema,
                                 true);

            return;
        }

        if (dto.StringComparisonType == StringComparisonType.Regex)
        {
            if (!IsValidRegex(dto.RegexString))
                context.AddNodeError(workflowId,
                                     nodeId,
                                     $"""
                                     {initialMessage}
                                     The comparison type is {nameof(StringComparisonType.Regex)} but the value for {nameof(dto.RegexString)} is not a valid regex string."
                                     """,
                                     WorkflowErrorType.Config,
                                     true);

            return;
        }

        if ((dto.StringComparisonType == StringComparisonType.Match
            || dto.StringComparisonType == StringComparisonType.MatchNoCase
            || dto.StringComparisonType == StringComparisonType.Contains
            || dto.StringComparisonType == StringComparisonType.ContainsNoCase)
            && string.IsNullOrWhiteSpace(dto.MatchString))
        {
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"{initialMessage} The comparison type is {nameof(StringComparisonType.Regex)} but the value for {nameof(dto.RegexString)} is not a valid regex string.",
                                 WorkflowErrorType.Config,
                                 true);

            return;
        }
    }

    private static bool IsValidRegex(string? pattern)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            return false;

        try
        {
            _ = new Regex(pattern);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}