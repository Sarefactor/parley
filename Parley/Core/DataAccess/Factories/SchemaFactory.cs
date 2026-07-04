using Parley.Configuration.Exceptions;
using Parley.Core.DataAccess.Enums;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Core.DataAccess.Models.Validation;
using Parley.Core.DataAccess.Models.Variables;
using Parley.Core.DataAccess.Repositories;
using Parley.Dtos.Schema;
using Parley.Mappers.Extensions;
using Parley.Validation;
using Parley.Workflows.Nodes.Factories;

namespace Parley.Core.DataAccess.Factories;

public class SchemaFactory : ISchemaFactory
{
    private readonly IParleyNodeFactory _parleyNodeFactory;
    private readonly IAgentSchemaRepository _agentSchemaRepository;

    public SchemaFactory(IParleyNodeFactory parleyNodeFactory,
                         IAgentSchemaRepository agentSchemaRepository)
    {
        _parleyNodeFactory = parleyNodeFactory;
        _agentSchemaRepository = agentSchemaRepository;
    }

    public async Task Upsert(AgentSchemaDto agentSchemaDto)
    {
        await BuildSchema(agentSchemaDto);
    }

    public async Task BuildSchema(AgentSchemaDto agentSchemaDto)
    {
        var context = new ParleyValidationContext();

        var agentSchema = CreateAgentSchema(agentSchemaDto, context);

        var workflows = BuildWorkflowSchemas(agentSchemaDto.WorkflowSchemas, context);
        agentSchema.SetWorkflowSchemas(workflows);

        if(context.HasErrors)
        {
            throw new ParleyValidationException(context.MapValidationDto());
        }

        var test = agentSchema.WorkflowSchemas.SelectMany(x => x.Nodes.Values).Where(x => x.Options.ValueKind == System.Text.Json.JsonValueKind.Undefined).ToList();

        _agentSchemaRepository.Upsert(agentSchema);
    }

    private AgentSchema CreateAgentSchema(AgentSchemaDto agentSchemaDto, ParleyValidationContext context)
    {
        AgentSchemaValidator.CollectValidationErrors(agentSchemaDto, context);

        return new AgentSchema(agentSchemaDto.Id,
                               agentSchemaDto.Name,
                               agentSchemaDto.Instructions);
    }

    private List<WorkflowSchema> BuildWorkflowSchemas(List<WorkflowSchemaDto> workflowSchemaDtos,
                                                      ParleyValidationContext context)
    {
        return workflowSchemaDtos.Select(x =>
        {
            WorkflowSchemaValidator.CollectValidationErrors(x,
                                                            x.ExecutionNodeId,
                                                            context);

            var workflow = new WorkflowSchema(x.ExecutionNodeId,
                                              x.Name,
                                              x.Intent,
                                              x.Description);

            var workflowVariables = BuildWorkflowVariables(x.WorkflowVariables,
                                                           x.ExecutionNodeId,
                                                           null,
                                                           WorkflowVariableType.Schema,
                                                           context);

            workflow.SetWorkflowVariables(workflowVariables);

            var workflowNodes = BuildWorkflowNodes(x,
                                                   context);

            workflow.SetWorkflowNodes(workflowNodes);

            return workflow;
        }).ToList();
    }

    public List<WorkflowVariable> BuildWorkflowVariables(List<WorkflowVariableDto> workflowVariableDtos,
                                                         Guid workflowId,
                                                         Guid? nodeId,
                                                         WorkflowVariableType workflowVariableType,
                                                         ParleyValidationContext context)
    {
        return workflowVariableDtos.Select(x =>
        {
            WorkflowVariableValidator.CollectValidationErrors(x,
                                                              workflowId,
                                                              nodeId,
                                                              workflowVariableType,
                                                              context);

            var workflowVariable = new WorkflowVariable(x.Name,
                                                        x.Description,
                                                        x.Type,
                                                        x.IsList,
                                                        x.IsNullable);

            var parleyVariables = x.ObjectVariables.Select(x =>
            {
                var parleyVariable = new ParleyVariable(x.Name,
                                                        x.Description,
                                                        x.Type,
                                                        x.IsList,
                                                        x.IsNullable);

                return parleyVariable;

            }).ToList();

            workflowVariable.SetObjectVariables(parleyVariables);

            return workflowVariable;
        }).ToList();
    }

    public Dictionary<Guid, NodeConfig> BuildWorkflowNodes(WorkflowSchemaDto schemaDto,
                                                           ParleyValidationContext context)
    {
        return schemaDto.Nodes.Select(x =>
        {
            var nodeConfig = new NodeConfig(x.NodeId,
                                            x.NodeType,
                                            x.PrimaryTransitionNode,
                                            x.SecondaryTransitionNode);

            var nodeVariables = BuildWorkflowVariables(x.NodeVariables,
                                                       schemaDto.ExecutionNodeId,
                                                       x.NodeId,
                                                       WorkflowVariableType.Node,
                                                       context);

            nodeConfig.SetNodeVariables(nodeVariables);

            var transitions = BuildTransitions(schemaDto, x.NodeId, x.Transitions, context);
            nodeConfig.SetTransitions(transitions);

            var nodePosition = new NodePosition(x.Position.X, x.Position.Y);
            nodeConfig.SetNodePosition(nodePosition);

            var variables = schemaDto.Nodes.SelectMany(x => x.NodeVariables)
                               .Concat(schemaDto.WorkflowVariables)
                               .ToList()
                               .AsReadOnly();

            var validationRules = BuildValidationRules(x.ValidationRules);
            nodeConfig.SetValidationRules(validationRules);

            if (ValidateNodeConfigOptions(schemaDto.ExecutionNodeId,
                                          x,
                                          schemaDto.Nodes.SelectMany(x => x.NodeVariables)
                                                         .Concat(schemaDto.WorkflowVariables)
                                                         .ToList()
                                                         .AsReadOnly(),
                                          context))
            {
                nodeConfig.SetOptions(x.NodeOptions);
            }

            return new KeyValuePair<Guid, NodeConfig>(x.NodeId, nodeConfig);
        }).ToDictionary();
    }

    private bool ValidateNodeConfigOptions(Guid workflowId,
                                           NodeConfigDto nodeConfigDto,
                                           IReadOnlyCollection<WorkflowVariableDto> workflowVariables,
                                           ParleyValidationContext context)
    {
        var validator = _parleyNodeFactory.GetNodeValidator(nodeConfigDto.NodeType);

        return validator.Validate(workflowId,
                                  nodeConfigDto,
                                  workflowVariables,
                                  context);
    }



    public List<Transition> BuildTransitions(WorkflowSchemaDto schemaDto,
                                             Guid nodeId,
                                             List<TransitionDto> transitionDtos,
                                             ParleyValidationContext context)
    {
        TransitionValidator.CollectValidationErrors(schemaDto.ExecutionNodeId,
                                                    nodeId,
                                                    transitionDtos,
                                                    context);
        
        return transitionDtos.Select(x =>
        {
            TransitionValidator.CollectValidationErrors(schemaDto,
                                                        nodeId,
                                                        x,
                                                        context);

            var transition = new Transition(x.Priority,
                                            x.TargetNodeId);

            var transitionRules = BuildTransitionRules(x.TransitionRules);
            transition.SetTransitionRules(transitionRules);

            return transition;
        }).ToList();
    }

    public List<TransitionRule> BuildTransitionRules(List<TransitionRuleDto> transitionRuleDtos)
    {
        return transitionRuleDtos.Select(x =>
        {
            return new TransitionRule(x.TargetKey,
                                      x.StringComparisonType,
                                      x.MatchString,
                                      x.RegexString,
                                      x.NumberComparisonType,
                                      x.MatchInt,
                                      x.BoolComparisonType,
                                      x.MatchBool,
                                      x.MatchDateTime);
        }).ToList();
    }

    public List<ValidationRule> BuildValidationRules(List<ValidationRuleDto> validationRules)
    {
        return validationRules.Select(x =>
        {
            return new ValidationRule(x.StringComparisonType,
                                      x.MatchString,
                                      x.RegexString,
                                      x.NumberComparisonType,
                                      x.MatchInt,
                                      x.BoolComparisonType,
                                      x.MatchBool,
                                      x.MatchDateTime);
        }).ToList();
    }
}