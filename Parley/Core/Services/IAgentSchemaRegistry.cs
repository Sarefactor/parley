using Parley.Dtos.Schema;
using Parley.Dtos.Search;

namespace Parley.Core.Services;

public interface IAgentSchemaRegistry
{
    Task<SearchResultDto<SchemaSearchItemDto>> Search(int skip,
                                                      int take);
    Task<AgentSchemaDto?> Get(Guid id);
    Task Delete(Guid id);
}