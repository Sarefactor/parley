using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Nodes;

namespace Parley.Workflows.State;

public interface IWorkflowStateManager
{
    Task InitialiseWorkflowVariables(IWorkflowContext context,
                                     List<WorkflowVariable> workflowVariables,
                                     JsonObject? extractedVariables,
                                     CancellationToken cancellationToken);

    Task<ICollection<WorkflowVariable>> GetWorkflowVariablesFromContext(IWorkflowContext context,
                                                                        CancellationToken cancellationToken);

    Task<WorkflowVariable> GetWorkflowVariable(IWorkflowContext context,
                                               string key,
                                               CancellationToken cancellationToken);

    Task SetWorkflowVariable(IWorkflowContext context,
                             WorkflowVariable workflowVariable,
                             CancellationToken cancellationToken);

    Task SetWorkflowVariables(IWorkflowContext context,
                              List<string> workflowVariables,
                              JsonObject extractedVariables,
                              CancellationToken cancellationToken);

    Task<IterationContext> GetIterationContext(Guid iteratorKey,
                                               string targetKey,
                                               IWorkflowContext context,
                                               CancellationToken cancellationToken);

    Task<(IterationContext? primaryContext, IterationContext? secondaryContext)> GetWorkflowVariableContexts(string primaryKey,
                                                                                                             string? secondaryKey,
                                                                                                             IWorkflowContext context,
                                                                                                             CancellationToken cancellationToken);

    Task SetIterationContext(IterationContext iterationContext,
                             IWorkflowContext workflowContext,
                             CancellationToken cancellationToken);

    Task ClearIterationContext(IterationContext iterationContext,
                               IWorkflowContext workflowContext,
                               CancellationToken cancellationToken);
}