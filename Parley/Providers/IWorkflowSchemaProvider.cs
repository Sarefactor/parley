using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Providers;

public interface IWorkflowSchemaProvider
{
    Task<WorkflowSchema> Provide(Guid executionNodeId);
}