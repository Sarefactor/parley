using Parley.Dtos.Schema;
using Parley.Validation;

namespace Parley.Core.DataAccess.Models.Schemas;

public static class AgentSchemaValidator
{
    public static void CollectValidationErrors(AgentSchemaDto dto, 
                                               ParleyValidationContext context)
    {
        if (dto.Id == Guid.Empty)
            context.AddAgentError($"Invalid value for {nameof(AgentSchema)} property {nameof(AgentSchema.AgentSchemaId)}.");

        if (string.IsNullOrWhiteSpace(dto.Name))
            context.AddAgentError($"Invalid value for {nameof(AgentSchema)} property {nameof(AgentSchema.Name)}.");

        if (string.IsNullOrWhiteSpace(dto.Instructions))
            context.AddAgentError($"Invalid value for {nameof(AgentSchema)} property {nameof(AgentSchema.Instructions)}.");
    }
}