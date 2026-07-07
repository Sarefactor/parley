using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public interface IWorkflowSchemaRepository
{
    Task<SearchResult<WorkflowSchema>> GetRange(int skip, int take);
    Task<WorkflowSchema?> Get(Guid id);
    Task Delete(Guid id);
    void Upsert(WorkflowSchema workflowSchema);
}