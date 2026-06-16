using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Nodes;

namespace Parley.Workflows.State;

public interface IWorkflowStateManager
{
    Task InitialiseWorkflowVariables(IWorkflowContext context,
                                     List<WorkflowVariable> workflowVariables,
                                     JsonObject extractedVariables,
                                     CancellationToken cancellationToken);

    Task<ICollection<WorkflowVariable>> GetWorkflowVariablesFromContext(IWorkflowContext context,
                                                                        CancellationToken cancellationToken);

    Task<WorkflowVariable> GetWorkflowVariable(IWorkflowContext context,
                                               string key,
                                               CancellationToken cancellationToken);

    Task SetWorkflowVariable(IWorkflowContext context,
                             WorkflowVariable workflowVariable,
                             CancellationToken cancellationToken);

    Task SetWorkflowVariable(IWorkflowContext context,
                             string variableKey,
                             object value,
                             CancellationToken cancellationToken);

    Task SetWorkflowVariables(IWorkflowContext context,
                              List<WorkflowVariable> workflowVariables,
                              CancellationToken cancellationToken);

    Task SetWorkflowVariables(IWorkflowContext context,
                              List<string> workflowVariables,
                              JsonObject extractedVariables,
                              CancellationToken cancellationToken);
}