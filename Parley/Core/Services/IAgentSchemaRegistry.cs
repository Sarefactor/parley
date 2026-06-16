
using Parley.Core.DataAccess.Repositories;
using Parley.Dtos.Schema;
using Parley.Dtos.Search;
using Parley.Mappers;
using TypeGen.Core.TypeAnnotations;

namespace Parley.Core.Services;

public interface IAgentSchemaRegistry
{
    Task<SearchResultDto<AgentSchemaSearchItemDto>> Search(int skip, int take);
    Task<AgentSchemaDto?> Get(Guid id);
    Task Delete(Guid id);
}

public class AgentSchemaRegistry : IAgentSchemaRegistry
{
    private readonly IAgentSchemaRepository _agentSchemaRepository;
    private readonly IAgentSchemaDtoMapper _agentSchemaDtoMapper;

    public AgentSchemaRegistry(IAgentSchemaRepository agentSchemaRepository,
                               IAgentSchemaDtoMapper agentSchemaDtoMapper)
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

    public async Task<SearchResultDto<AgentSchemaSearchItemDto>> Search(int skip, int take)
    {
        var result = await _agentSchemaRepository.GetRange(skip, take);

        return new SearchResultDto<AgentSchemaSearchItemDto>
        {
            TotalResults = result.TotalResults,
            Page = result.Page,
            PageSize = result.PageSize,
            Results = result.Results.Select(x => new AgentSchemaSearchItemDto
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

[ExportTsClass]
public class SearchResultDto<T>
{
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T> Results { get; set; } = [];
}