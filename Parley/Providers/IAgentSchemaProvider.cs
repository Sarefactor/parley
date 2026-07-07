using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Providers;

public interface IAgentSchemaProvider
{
    Task<AgentSchema> Provide();

    Task SetActiveSchema(Guid agentSchemaId);
}