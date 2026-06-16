using Microsoft.Agents.AI;

namespace Parley.Providers;

public interface ISessionProvider
{
    Task<AgentSession> GetAgentSessionAsync(AIAgent agent,
                                            AgentRunOptions agentRunOptions,
                                            CancellationToken cancellationToken);
}