using Parley.Dtos.Schema;

namespace Parley.Core.DataAccess.Factories;

public interface ISchemaFactory
{
    Task Upsert(AgentSchemaDto agentSchemaDto);
    Task Upsert(WorkflowSchemaDto workflowSchemaDto);
}