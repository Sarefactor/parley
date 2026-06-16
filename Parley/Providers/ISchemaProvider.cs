using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Providers;

public interface ISchemaProvider
{
    Task<AgentSchema> Provide();

    Task SetActiveSchema(Guid agentSchemaId);
}