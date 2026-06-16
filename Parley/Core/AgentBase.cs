using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Parley.Providers;
using System.Runtime.CompilerServices;

namespace Parley.Core;

internal class AgentBase : DelegatingAIAgent
{
    private readonly ISessionProvider _sessionProvider;

    public AgentBase(AIAgent innerAgent, ISessionProvider sessionProvider)
        : base(innerAgent)
    {
        _sessionProvider = sessionProvider;
    }

    protected override async IAsyncEnumerable<AgentResponseUpdate> RunCoreStreamingAsync(IEnumerable<ChatMessage> messages,
                                                                                         AgentSession? session = null,
                                                                                         AgentRunOptions? options = null,
                                                                                         [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var storedSession = await _sessionProvider.GetAgentSessionAsync(this, options ?? new AgentRunOptions(), cancellationToken);

        await foreach (var update in base.RunCoreStreamingAsync(messages, storedSession, options, cancellationToken)
                                         .WithCancellation(cancellationToken))
        {
            yield return update;
        }
    }
}