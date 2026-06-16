using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public interface IAgentSchemaRepository
{
    Task<SearchResult<AgentSchema>> GetRange(int skip, int take);
    Task<AgentSchema?> Get(Guid id);
    Task Delete(Guid id);
    void Upsert(AgentSchema agentSchema);
}