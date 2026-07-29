using Parley.Dtos.Schema;
using Parley.Dtos.Search;

namespace Parley.Core.Services;

public interface IWorkflowSchemaRegistry
{
    Task<SearchResultDto<SchemaSearchItemDto>> Search(int skip,
                                                      int take);
    Task<WorkflowSchemaDto?> Get(Guid id);
    Task Delete(Guid id);
}