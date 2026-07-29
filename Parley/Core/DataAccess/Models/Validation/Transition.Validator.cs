using Parley.Core.DataAccess.Models.Validation.RuleEvaluators;
using Parley.Core.Enums;
using Parley.Dtos.Schema;
using Parley.Validation;
using Parley.Validation.Enums;

namespace Parley.Core.DataAccess.Models.Validation;

public static class TransitionValidator
{
    public static void CollectValidationErrors(Guid workflowId,
                                               Guid nodeId,
                                               List<TransitionDto> transitionDtos,
                                               ParleyValidationContext context)
    {
        var priorityList = transitionDtos.Select(x => x.Priority)
                                         .GroupBy(x => x)
                                         .ToList();

        if (priorityList.Any(x => x.Count() > 1))
            context.AddNodeError(workflowId,
                                 nodeId,
                                 $"Node has multiple transitions with matching {nameof(Transition.Priority)} values",
                                 WorkflowErrorType.Schema,
                                 false);

        var targetNodeList = transitionDtos.Select(x => x.TargetNodeId)
                                           .GroupBy(x => x)
                                           .ToList();

        foreach (var targetNode in targetNodeList)
        {
            if (targetNode.Count() > 1)
                context.AddNodeError(workflowId,
                                     nodeId,
                                     $"Node has multiple transitions targeting the node: {targetNode.Key}",
                                     WorkflowErrorType.Schema,
                                     false);
        }
    }

    public static void CollectValidationErrors(WorkflowSchemaDto schemaDto,
                                               Guid nodeId,
                                               TransitionDto dto,
                                               ParleyValidationContext context)
    {
        if (dto.Priority <= 0)
            context.AddNodeError(schemaDto.ExecutionNodeId,
                                 nodeId,
                                 $"The priority on transition connecting to node: {dto.TargetNodeId} is invalid. Must be set to a value greater than 0.",
                                 WorkflowErrorType.Config,
                                 false);

        if (dto.TargetNodeId == Guid.Empty)
            context.AddNodeError(schemaDto.ExecutionNodeId,
                                 nodeId,
                                 $"Node has a transition targeting an empty {nameof(Guid)}. Good luck fixing this one.",
                                 WorkflowErrorType.Config,
                                 false);

        var variables = schemaDto.Nodes.SelectMany(x => x.NodeVariables)
                                       .Concat(schemaDto.WorkflowVariables)
                                       .ToList()
                                       .AsReadOnly();

        foreach(var transitionRule in dto.TransitionRules)
        {
            var variable = transitionRule.TargetKey.Contains(':')
                           ? GetObjectVariableFromDto(transitionRule.TargetKey, variables)
                           : variables.FirstOrDefault(x => x.Name == transitionRule.TargetKey) as ParleyVariableDto;

            if (variable == null)
            {
                context.AddNodeError(schemaDto.ExecutionNodeId,
                                     nodeId,
                                     $"The transition targeting node: {dto.TargetNodeId} evaluates against a variable ({transitionRule.TargetKey}) that does not exist in either the workflow or node configuration.",
                                     WorkflowErrorType.Schema,
                                     true);

                continue;
            }

            if (!Enum.IsDefined(variable.Type))
            {
                context.AddNodeError(schemaDto.ExecutionNodeId,
                                     nodeId,
                                     $"The transition targeting node: {dto.TargetNodeId} evaluates against a variable ({transitionRule.TargetKey}) with an incorrect value set for {nameof(VariableDataType)}.",
                                     WorkflowErrorType.Schema,
                                     true);

                continue;
            }

            switch (variable.Type)
            {
                case VariableDataType.String:
                    StringValidationRuleEvaluator.ValidateForTransition(schemaDto.ExecutionNodeId,
                                                                              nodeId,
                                                                              dto.TargetNodeId,
                                                                              variable.Name,
                                                                              transitionRule,
                                                                              context);
                    break;

                case VariableDataType.Integer:
                    NumericalValidationRuleEvaluator.ValidateNumericalTransitionRule(schemaDto.ExecutionNodeId,
                                                                                    nodeId,
                                                                                    dto.TargetNodeId,
                                                                                    variable.Name,
                                                                                    transitionRule,
                                                                                    context);
                    break;

                case VariableDataType.DateTime:
                    DateTimeValidationRuleValidator.ValidateDateTimeTransitionRule(schemaDto.ExecutionNodeId,
                                                                                  nodeId,
                                                                                  dto.TargetNodeId,
                                                                                  variable.Name,
                                                                                  transitionRule,
                                                                                  context);
                    break;

                case VariableDataType.Bool:
                    BoolValidationRuleValidator.ValidateBoolTransitionRule(schemaDto.ExecutionNodeId,
                                                                          nodeId,
                                                                          dto.TargetNodeId,
                                                                          variable.Name,
                                                                          transitionRule,
                                                                          context);
                    break;
            }
        }
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