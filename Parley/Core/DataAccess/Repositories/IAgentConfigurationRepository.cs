using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public interface IAgentConfigurationRepository
{
    Task<AgentConfiguration?> Get();
    Task UpdateActiveConversationId(Guid schemaId);
}