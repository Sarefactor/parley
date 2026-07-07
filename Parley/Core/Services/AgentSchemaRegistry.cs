using Parley.Core.DataAccess.Repositories;
using Parley.Dtos.Schema;
using Parley.Dtos.Search;
using Parley.Mappers;

namespace Parley.Core.Services;

public class AgentSchemaRegistry : IAgentSchemaRegistry
{
    private readonly IAgentSchemaRepository _agentSchemaRepository;
    private readonly ISchemaDtoMapper _agentSchemaDtoMapper;

    public AgentSchemaRegistry(IAgentSchemaRepository agentSchemaRepository,
                               ISchemaDtoMapper agentSchemaDtoMapper)
    {
        _agentSchemaRepository = agentSchemaRepository;
        _agentSchemaDtoMapper = agentSchemaDtoMapper;
    }

    public async Task<AgentSchemaDto?> Get(Guid id)
    {
        var agentSchema = await _agentSchemaRepository.Get(id);

        if (agentSchema == null)
            return null;

        return _agentSchemaDtoMapper.Map(agentSchema);
    }

    public async Task<SearchResultDto<SchemaSearchItemDto>> Search(int skip, int take)
    {
        var result = await _agentSchemaRepository.GetRange(skip, take);

        return new SearchResultDto<SchemaSearchItemDto>
        {
            TotalResults = result.TotalResults,
            Page = result.Page,
            PageSize = result.PageSize,
            Results = result.Results.Select(x => new SchemaSearchItemDto
            {
                Id = x.AgentSchemaId,
                Name = x.Name,
                LastModified = x.LastModified
            }).ToList()
        };
    }

    public async Task Delete(Guid id)
    {
        await _agentSchemaRepository.Delete(id);
    }
}