using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Nodes;

namespace Parley.Workflows.State;

public class WorkflowStateManager : IWorkflowStateManager
{
    public const string WorkflowContextKey = "WorkflowVariables";

    public async Task InitialiseWorkflowVariables(IWorkflowContext context,
                                                  List<WorkflowVariable> workflowVariables,
                                                  JsonObject extractedVariables,
                                                  CancellationToken cancellationToken)
    {
        var variablesToStore = workflowVariables.Select(wv => new WorkflowVariable(wv, extractedVariables.FirstOrDefault(ev => ev.Key == wv.Name).Value))
                                                .ToList();

        await context.QueueStateUpdateAsync(WorkflowContextKey, variablesToStore, WorkflowContextKey, cancellationToken);
    }

    public async Task<ICollection<WorkflowVariable>> GetWorkflowVariablesFromContext(IWorkflowContext context,
                                                                                     CancellationToken cancellationToken)
    {
        return await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey, WorkflowContextKey, cancellationToken)
                     ?? throw new KeyNotFoundException($"{nameof(WorkflowVariable)} collection not found in the workflow context.");
    }

    public async Task<WorkflowVariable> GetWorkflowVariable(IWorkflowContext context,
                                                            string key,
                                                            CancellationToken cancellationToken)
    {
        var workflowVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey, WorkflowContextKey, cancellationToken);

        var workflowVariable = workflowVariables?.FirstOrDefault(x => x.Name == key);

        if (workflowVariable == null)
            throw new KeyNotFoundException($"{nameof(WorkflowVariable)} not found in the workflow context.");

        return workflowVariable;
    }

    public async Task SetWorkflowVariable(IWorkflowContext context,
                                          WorkflowVariable workflowVariable,
                                          CancellationToken cancellationToken)
        => await SetWorkflowVariables(context, [workflowVariable], cancellationToken);

    public async Task SetWorkflowVariable(IWorkflowContext context,
                                          string variableKey,
                                          object value,
                                          CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey, WorkflowContextKey, cancellationToken);

        if (contextVariables == null)
            throw new Exception("Error reading variables from context.");

        var contextVariable = contextVariables.First(x => x.Name == variableKey);

        contextVariable.SetValue(value);
    }

    public async Task SetWorkflowVariables(IWorkflowContext context,
                                           List<WorkflowVariable> workflowVariables,
                                           CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey, WorkflowContextKey, cancellationToken);

        foreach (var workflowVariable in workflowVariables)
        {
            var contextVariable = contextVariables?.FirstOrDefault(x => x.Name == workflowVariable.Name);

            if (contextVariable == null)
                throw new KeyNotFoundException($"{nameof(WorkflowVariable)} not found in the workflow context.");

            if (workflowVariable.Value == null)
                continue;

            contextVariable.SetValue(workflowVariable.Value);
        }

        await context.QueueStateUpdateAsync(WorkflowContextKey, contextVariables, WorkflowContextKey, cancellationToken);
    }

    public async Task SetWorkflowVariables(IWorkflowContext context,
                                           List<string> workflowVariables,
                                           JsonObject extractedVariables,
                                           CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey, WorkflowContextKey, cancellationToken);

        foreach (var workflowVariable in workflowVariables)
        {
            var contextVariable = contextVariables?.FirstOrDefault(x => x.Name == workflowVariable);

            if (contextVariable == null)
                throw new KeyNotFoundException($"{nameof(WorkflowVariable)} not found in the workflow context.");

            var value = extractedVariables.FirstOrDefault(ev => ev.Key == workflowVariable).Value;

            if (value == null)
                continue;

            contextVariable.SetValue(value);
        }

        await context.QueueStateUpdateAsync(WorkflowContextKey, contextVariables, WorkflowContextKey, cancellationToken);
    }
}