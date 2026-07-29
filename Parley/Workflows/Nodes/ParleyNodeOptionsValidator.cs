using Parley.Core.DataAccess.Models.Validation.RuleEvaluators;
using Parley.Core.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using System.Text.Json;

namespace Parley.Workflows.Nodes;

public abstract class ParleyNodeOptionsValidator
{
    public abstract string NodeType { get; }

    //TODO: Validation of node options and validation rules on the node level
    //public abstract bool Validate(string nodeType,
    //                              object? options,
    //                              List<ValidationRuleDto> validationRules,
    //                              IReadOnlyCollection<WorkflowVariableDto> workflowVariables,
    //                              ParleyValidationContext context);

    public abstract bool Validate(Guid workflowId,
                                  NodeConfigDto dto,
                                  IReadOnlyCollection<WorkflowVariableDto> workflowVariables,
                                  ParleyValidationContext context);

    protected bool TrySerialiseOptions<TOptions>(object? options, out TOptions? convertedOptions)
    {
        if (options is not JsonElement element)
        {
            convertedOptions = default;
            return false;
        }

        try
        {
            convertedOptions = element.Deserialize<TOptions>();

            if (convertedOptions == null)
                return (false);

            return true;
        }
        catch (JsonException)
        {
            convertedOptions = default;
            return false;
        }
    }

    protected void ValidateValidationRules(Guid workflowId,
                                           NodeConfigDto nodeConfigDto,                                 
                                           ParleyVariableDto targetVariable,
                                           ParleyValidationContext context)
    {
        foreach (var validationRule in nodeConfigDto.ValidationRules)
        {
            switch (targetVariable.Type)
            {
                case VariableDataType.String:
                    StringValidationRuleEvaluator.Validate(workflowId,
                                                           nodeConfigDto.NodeId,
                                                           targetVariable.Name,
                                                           validationRule,
                                                           context);
                    break;

                case VariableDataType.Integer:
                    StringValidationRuleEvaluator.Validate(workflowId,
                                                           nodeConfigDto.NodeId,
                                                           targetVariable.Name,
                                                           validationRule,
                                                           context);
                    break;

                case VariableDataType.DateTime:
                    StringValidationRuleEvaluator.Validate(workflowId,
                                                           nodeConfigDto.NodeId,
                                                           targetVariable.Name,
                                                           validationRule,
                                                           context);
                    break;

                case VariableDataType.Bool:
                    StringValidationRuleEvaluator.Validate(workflowId,
                                                           nodeConfigDto.NodeId,
                                                           targetVariable.Name,
                                                           validationRule,
                                                           context);
                    break;
            }
        }
    }

    protected ParleyVariableDto? GetParleyVariableDto(string? targetKey,
                                                      IReadOnlyCollection<WorkflowVariableDto> variables)
    {
        if (string.IsNullOrWhiteSpace(targetKey))
            return null;

        return targetKey.Contains(':') ? GetObjectVariableFromDto(targetKey, variables)
                                       : variables.FirstOrDefault(x => x.Name == targetKey) as ParleyVariableDto;
    }

    private static ParleyVariableDto? GetObjectVariableFromDto(string targetKey,
                                                               IReadOnlyCollection<WorkflowVariableDto> variables)
    {
        if (!targetKey.Contains(':'))
            return null;

        var splitKey = targetKey.Split(':');

        var variable = variables.FirstOrDefault(x => x.Name == splitKey[0]);

        if (variable == null)
            return null;

        return variable.ObjectVariables.FirstOrDefault(x => x.Name == splitKey[1]);
    }
}