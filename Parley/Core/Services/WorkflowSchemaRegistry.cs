using Parley.Core.DataAccess.Repositories;
using Parley.Dtos.Schema;
using Parley.Dtos.Search;
using Parley.Mappers;

namespace Parley.Core.Services;

public class WorkflowSchemaRegistry : IWorkflowSchemaRegistry
{
    private readonly IWorkflowSchemaRepository _workflowSchemaRepository;
    private readonly ISchemaDtoMapper _schemaDtoMapper;

    public WorkflowSchemaRegistry(IWorkflowSchemaRepository workflowSchemaRepository,
                                  ISchemaDtoMapper schemaDtoMapper)
    {
        _workflowSchemaRepository = workflowSchemaRepository;
        _schemaDtoMapper = schemaDtoMapper;
    }

    public async Task<WorkflowSchemaDto?> Get(Guid id)
    {
        var schema = await _workflowSchemaRepository.Get(id);

        if (schema == null)
            return null;

        return _schemaDtoMapper.Map(schema);
    }

    public async Task<SearchResultDto<SchemaSearchItemDto>> Search(int skip, int take)
    {
        var result = await _workflowSchemaRepository.GetRange(skip, take);

        return new SearchResultDto<SchemaSearchItemDto>
        {
            TotalResults = result.TotalResults,
            Page = result.Page,
            PageSize = result.PageSize,
            Results = result.Results.Select(x => new SchemaSearchItemDto
            {
                Id = x.ExecutionNodeId,
                Name = x.Name,
                LastModified = DateTime.UtcNow
            }).ToList()
        };
    }

    public async Task Delete(Guid id)
    {
        await _workflowSchemaRepository.Delete(id);
    }
}