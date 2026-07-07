using Parley.Core.DataAccess.Models.Schemas;
using Parley.Core.DataAccess.Repositories;

namespace Parley.Providers;

internal class WorkflowSchemaProvider : IWorkflowSchemaProvider
{
    private readonly IWorkflowSchemaRepository _workflowSchemaRepository;

    public WorkflowSchemaProvider(IWorkflowSchemaRepository workflowSchemaRepository)
    {
        _workflowSchemaRepository = workflowSchemaRepository;
    }

    public async Task<WorkflowSchema> Provide(Guid executionNodeId)
    {
        var workflowSchema = await _workflowSchemaRepository.Get(executionNodeId);

        if (workflowSchema == null)
            throw new Exception("Could not get an agent schema from the db.");

        return workflowSchema;
    }
}
