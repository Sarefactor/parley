using Microsoft.Agents.AI.Workflows;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Nodes;

namespace Parley.Workflows.State;

public class WorkflowStateManager : IWorkflowStateManager
{
    public const string WorkflowContextKey = "WorkflowVariables";
    public const string IterationStoreKey = nameof(IterationStoreKey);

    public async Task InitialiseWorkflowVariables(IWorkflowContext context,
                                                  List<WorkflowVariable> workflowVariables,
                                                  JsonObject? extractedVariables,
                                                  CancellationToken cancellationToken)
    {
        var variablesToStore = workflowVariables.Select(wv => new WorkflowVariable(wv,
                                                                                   extractedVariables?.FirstOrDefault(ev => ev.Key == wv.Name).Value))
                                                .ToList();

        await context.QueueStateUpdateAsync(WorkflowContextKey,
                                            variablesToStore,
                                            WorkflowContextKey,
                                            cancellationToken);
    }

    public async Task<ICollection<WorkflowVariable>> GetWorkflowVariablesFromContext(IWorkflowContext context,
                                                                                     CancellationToken cancellationToken)
    {
        return await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey,
                                                                    WorkflowContextKey,
                                                                    cancellationToken)
                     ?? throw new KeyNotFoundException($"{nameof(WorkflowVariable)} collection not found in the workflow context.");
    }

    public async Task<WorkflowVariable> GetWorkflowVariable(IWorkflowContext context,
                                                            string key,
                                                            CancellationToken cancellationToken)
    {
        var keyToAccess = key.Contains(':') ? key.Split(':')[0]
                                            : key;

        var workflowVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey,
                                                                                     WorkflowContextKey,
                                                                                     cancellationToken);

        var workflowVariable = workflowVariables?.FirstOrDefault(x => x.Name == keyToAccess);

        if (workflowVariable == null)
            throw new KeyNotFoundException($"{nameof(WorkflowVariable)} not found in the workflow context.");

        return workflowVariable;
    }

    public async Task SetWorkflowVariable(IWorkflowContext context,
                                          WorkflowVariable workflowVariable,
                                          CancellationToken cancellationToken)
        => await SetWorkflowVariables(context,
                                      [workflowVariable],
                                      cancellationToken);

    public async Task SetWorkflowVariable(IWorkflowContext context,
                                          string variableKey,
                                          object value,
                                          CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey,
                                                                                    WorkflowContextKey,
                                                                                    cancellationToken);

        if (contextVariables == null)
            throw new Exception("Error reading variables from context.");

        var contextVariable = contextVariables.First(x => x.Name == variableKey);

        contextVariable.SetValue(value);
    }

    public async Task SetWorkflowVariables(IWorkflowContext context,
                                           List<WorkflowVariable> workflowVariables,
                                           CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey,
                                                                                    WorkflowContextKey,
                                                                                    cancellationToken);

        foreach (var workflowVariable in workflowVariables)
        {
            var contextVariable = contextVariables?.FirstOrDefault(x => x.Name == workflowVariable.Name);

            if (contextVariable == null)
                throw new KeyNotFoundException($"{nameof(WorkflowVariable)} not found in the workflow context.");

            if (workflowVariable.Value == null)
                continue;

            contextVariable.SetValue(workflowVariable.Value);
        }

        await context.QueueStateUpdateAsync(WorkflowContextKey,
                                            contextVariables,
                                            WorkflowContextKey,
                                            cancellationToken);
    }

    public async Task SetWorkflowVariables(IWorkflowContext context,
                                           List<string> workflowVariables,
                                           JsonObject extractedVariables,
                                           CancellationToken cancellationToken)
    {
        var contextVariables = await context.ReadStateAsync<List<WorkflowVariable>>(WorkflowContextKey,
                                                                                    WorkflowContextKey,
                                                                                    cancellationToken);

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

        await context.QueueStateUpdateAsync(WorkflowContextKey,
                                            contextVariables,
                                            WorkflowContextKey,
                                            cancellationToken);
    }

    public async Task<IterationContext> GetIterationContext(Guid iteratorKey,
                                                            string targetKey,
                                                            IWorkflowContext context,
                                                            CancellationToken cancellationToken)
    {
        var iterationStore = await GetIterationStore(context, cancellationToken);

        var existingContext = iterationStore.FirstOrDefault(x => x.Key == iteratorKey).Value;

        if (existingContext != null)
            return existingContext with { IsNew = false };

        var iterationContext = new IterationContext(iteratorKey,
                                                    targetKey);

        iterationStore.Add(iteratorKey,
                           iterationContext);

        await context.QueueStateUpdateAsync(IterationStoreKey,
                                            iterationStore,
                                            IterationStoreKey,
                                            cancellationToken);

        var iterationStoreDebug = await GetIterationStore(context, cancellationToken);

        return iterationContext;
    }

    public async Task<(IterationContext? primaryContext, IterationContext? secondaryContext)> GetWorkflowVariableContexts(string primaryKey,
                                                                                                                          string? secondaryKey,
                                                                                                                          IWorkflowContext context,
                                                                                                                          CancellationToken cancellationToken)
    {
        var iterationStore = await GetIterationStore(context, cancellationToken);

        return (iterationStore.FirstOrDefault(x => x.Value.TargetKey == primaryKey).Value,
                secondaryKey != null ? iterationStore.FirstOrDefault(x => x.Value.TargetKey == secondaryKey).Value : null);
    }

    public async Task SetIterationContext(IterationContext iterationContext,
                                          IWorkflowContext workflowContext,
                                          CancellationToken cancellationToken)
    {
        var iterationStore = await GetIterationStore(workflowContext,
                                                     cancellationToken);

        iterationStore[iterationContext.IteratorId] = iterationContext;
    }

    public async Task ClearIterationContext(IterationContext iterationContext,
                                            IWorkflowContext workflowContext,
                                            CancellationToken cancellationToken)
    {
        var iterationStore = await GetIterationStore(workflowContext,
                                                     cancellationToken);

        if (iterationStore.ContainsKey(iterationContext.IteratorId))
            iterationStore.Remove(iterationContext.IteratorId);
    }

    private async Task<Dictionary<Guid, IterationContext>> GetIterationStore(IWorkflowContext context,
                                                                             CancellationToken cancellationToken)
    {
        return await context.ReadOrInitStateAsync(IterationStoreKey,
                                                  () => new Dictionary<Guid, IterationContext>(),
                                                  IterationStoreKey,
                                                  cancellationToken);
    }
}