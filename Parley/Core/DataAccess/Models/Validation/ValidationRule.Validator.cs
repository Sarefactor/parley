using Parley.Core.DataAccess.Models.Validation.RuleEvaluators;
using Parley.Dtos.Schema;
using Parley.Validation;

namespace Parley.Core.DataAccess.Models.Validation;

public static class ValidationRuleValidator
{
    public static void ValidateStringValidationRule(Guid workflowId,
                                                    Guid nodeId,
                                                    string variableName,
                                                    ValidationRuleDto dto,
                                                    ParleyValidationContext context)
    {
        StringValidationRuleEvaluator.Validate(workflowId,
                                               nodeId,
                                               variableName,
                                               dto,
                                               context);
    }

    public static void ValidateNumericalValidationRule(Guid workflowId,
                                                       Guid nodeId,
                                                       string variableName,
                                                       ValidationRuleDto dto,
                                                       ParleyValidationContext context)
    {
        NumericalValidationRuleEvaluator.ValidateNumericalValidationRule(workflowId,
                                                                         nodeId,
                                                                         variableName,
                                                                         dto,
                                                                         context);
    }

    public static void ValidateDateTimeValidationRule(Guid workflowId,
                                                      Guid nodeId,
                                                      string variableName,
                                                      ValidationRuleDto dto,
                                                      ParleyValidationContext context)
    {
        DateTimeValidationRuleValidator.ValidateDateTimeValidationRule(workflowId,
                                                                       nodeId,
                                                                       variableName,
                                                                       dto,
                                                                       context);
    }

    public static void ValidateBoolValidationRule(Guid workflowId,
                                                  Guid nodeId,
                                                  string variableName,
                                                  ValidationRuleDto dto,
                                                  ParleyValidationContext context)
    {
        BoolValidationRuleValidator.ValidateBoolValidationRule(workflowId,
                                                               nodeId,
                                                               variableName,
                                                               dto,
                                                               context);
    }
}