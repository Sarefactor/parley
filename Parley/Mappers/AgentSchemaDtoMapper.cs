using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Dtos.Schema;

namespace Parley.Mappers;

public interface ISchemaDtoMapper
{
    AgentSchemaDto Map(AgentSchema agentSchema);
    WorkflowSchemaDto Map(WorkflowSchema workflowSchema);
}

public class AgentSchemaDtoMapper : ISchemaDtoMapper
{
    public AgentSchemaDto Map(AgentSchema agentSchema)
    {
        return new AgentSchemaDto
        {
            Id = agentSchema.AgentSchemaId,
            Name = agentSchema.Name,
            Instructions = agentSchema.Instructions,
            WorkflowSchemas = agentSchema.WorkflowSchemas.Select(MapWorkflowSchemaDto)
                                                         .ToList()
        };
    }

    public WorkflowSchemaDto Map(WorkflowSchema workflowSchema)
    {
        return MapWorkflowSchemaDto(workflowSchema);
    }

    private static WorkflowSchemaDto MapWorkflowSchemaDto(WorkflowSchema schema)
    {
        return new WorkflowSchemaDto
        {
            Name = schema.Name,
            Intent = schema.Intent,
            Description = schema.Description,
            ExecutionNodeId = schema.ExecutionNodeId,
            Nodes = schema.Nodes.Values.Select(NodeConfigDto).ToList(),
            WorkflowVariables = schema.WorkflowVariables.Select(MapWorkflowVariableDto).ToList()
        };
    }

    private static NodeConfigDto NodeConfigDto(NodeConfig node)
    {
        return new NodeConfigDto
        {
            NodeId = node.NodeId,
            NodeType = node.NodeType,
            PrimaryTransitionNode = node.PrimaryTransitionNode,
            SecondaryTransitionNode = node.SecondaryTransitionNode,
            NodeOptions = node.Options,
            NodeVariables = node.NodeVariables.Select(MapWorkflowVariableDto).ToList(),
            Transitions = node.Transitions.Select(MapTransitionDto).ToList(),
            ValidationRules = node.ValidationRules.Select(MapValidationRuleDto).ToList(),
            Position = MapNodePositionDto(node.NodePosition)
        };
    }

    private static WorkflowVariableDto MapWorkflowVariableDto(WorkflowVariable variable)
    {
        return new WorkflowVariableDto
        {
            Name = variable.Name,
            Description = variable.Description,
            Type = variable.Type,
            IsList = variable.IsList,
            IsNullable = variable.Nullable,
            ObjectVariables = variable.ObjectVariables.Select(MapParleyVariableDto).ToList()
        };
    }

    private static ParleyVariableDto MapParleyVariableDto(ParleyVariable variable)
    {
        return new ParleyVariableDto
        {
            Name = variable.Name,
            Description = variable.Description,
            Type = variable.Type,
            IsList = variable.IsList,
            IsNullable = variable.Nullable
        };
    }

    private static TransitionDto MapTransitionDto(Transition transition)
    {
        return new TransitionDto
        {
            Priority = transition.Priority,
            TargetNodeId = transition.TargetNodeId,
            TransitionRules = transition.TransitionRules.Select(MapTransitionRuleDto).ToList()
        };
    }

    private static TransitionRuleDto MapTransitionRuleDto(TransitionRule rule)
    {
        return new TransitionRuleDto
        {
            TargetKey = rule.TargetKey,
            StringComparisonType = rule.StringComparisonType,
            MatchString = rule.MatchString,
            RegexString = rule.RegexString,
            NumberComparisonType = rule.NumberComparisonType,
            MatchInt = rule.MatchInt,
            BoolComparisonType = rule.BoolComparisonType,
            MatchBool = rule.MatchBool,
            MatchDateTime = rule.MatchDateTime
        };
    }

    private static ValidationRuleDto MapValidationRuleDto(ValidationRule rule)
    {
        return new ValidationRuleDto
        {
            StringComparisonType = rule.StringComparisonType,
            MatchString = rule.MatchString,
            RegexString = rule.RegexString,
            NumberComparisonType = rule.NumberComparisonType,
            MatchInt = rule.MatchInt,
            BoolComparisonType = rule.BoolComparisonType,
            MatchBool = rule.MatchBool,
            MatchDateTime = rule.MatchDateTime
        };
    }

    private static NodePositionDto MapNodePositionDto(NodePosition position)
    {
        return new NodePositionDto
        {
            X = position.X,
            Y = position.Y
        };
    }
}