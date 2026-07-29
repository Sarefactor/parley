using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Parley.Core;

namespace Parley.Providers;

public class AgentProvider : IAgentProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAgentSchemaProvider _schemaProvider;
    private readonly IChatClientProvider _chatClientProvider;

    private AIAgent? BaseAgent { get; set; }
    private IChatClient? ChatClient { get; set; }

    public AgentProvider(IServiceProvider serviceProvider,
                         IAgentSchemaProvider schemaProvider,
                         IChatClientProvider chatClientProvider)
                         
    {
        _serviceProvider = serviceProvider;
        _schemaProvider = schemaProvider;
        _chatClientProvider = chatClientProvider;
    }

    public async Task<AIAgent> CreateParleyAgent()
        => await CreateAiAgent();

    private async Task<AIAgent> CreateAiAgent()
    {
        ChatClient = _chatClientProvider.Provide();

        //await EnsureBaseAgentExists();

        return new AgentBase(BaseAgent!.AsBuilder()
                                       .Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null)
                                       .Build(),
                             _serviceProvider.GetRequiredService<ISessionProvider>());
    }

    //private async Task EnsureBaseAgentExists()
    //    => BaseAgent ??= await new BaseWorkflow(_schemaProvider).ConstructBaseWorkflow(ChatClient!);

    async Task<AgentResponse> CustomAgentRunMiddleware(
    IEnumerable<Microsoft.Extensions.AI.ChatMessage> messages,
    AgentSession? session,
    AgentRunOptions? options,
    AIAgent innerAgent,
    CancellationToken cancellationToken)
    {
        Console.WriteLine($"Input: {messages.Count()}");
        var response = await innerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);
        Console.WriteLine($"Output: {response.Messages.Count}");
        return response;
    }
}