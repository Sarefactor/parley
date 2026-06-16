using Microsoft.Agents.AI;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace Parley.Providers;

public class SessionProvider : ISessionProvider
{
    private readonly ConcurrentDictionary<string, AgentSession> _sessions = new();

    public async Task<AgentSession> GetAgentSessionAsync(AIAgent agent,
                                                         AgentRunOptions agentRunOptions,
                                                         CancellationToken cancellationToken)
    {
        ChatClientAgentRunOptions options = (ChatClientAgentRunOptions)agentRunOptions;

        var threadIdExists = options.ChatOptions!.AdditionalProperties!.TryGetValue<string>("ag_ui_thread_id", out var threadId);

        if (!threadIdExists)
            throw new ValidationException("No thread Id found");

        var sessionExists = _sessions.TryGetValue(threadId!, out var session);

        if (sessionExists && session != null)
            return session;

        session = await agent.CreateSessionAsync(cancellationToken);

        _sessions.TryAdd(threadId!, session);

        return session;
    }
}