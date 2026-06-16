using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Chat;
using Parley.Core;
using Parley.Workflows.Examples;

namespace Parley.Providers;

public class AgentProvider : IAgentProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISchemaProvider _schemaProvider;
    private readonly IChatClientProvider _chatClientProvider;

    private AIAgent? BaseAgent { get; set; }
    private IChatClient? ChatClient { get; set; }

    public AgentProvider(IServiceProvider serviceProvider,
                         ISchemaProvider schemaProvider,
                         IChatClientProvider chatClientProvider)
                         
    {
        _serviceProvider = serviceProvider;
        _schemaProvider = schemaProvider;
        _chatClientProvider = chatClientProvider;
    }

    public async Task<AIAgent> CreateParleyAgent()
        => await CreateAiAgent();
        //=> CreateAzureFoundryAgent();

    //private AIAgent CreateAzureFoundryAgent()
    //{
    //    AzureOpenAIClient client = new(new Uri(_configuration["Parley:AzureOpenAI:Endpoint"]!),
    //                                   new DefaultAzureCredential());

    //    ChatClient = client.GetChatClient(_configuration["Parley:AzureOpenAI:Deployment"]!);

    //    EnsureBaseAgentExists();

    //    return new AgentBase(BaseAgent!.AsBuilder().Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null).Build(),
    //                         _serviceProvider.GetRequiredService<ISessionProvider>());
    //}

    private async Task<AIAgent> CreateAiAgent()
    {
        ChatClient = _chatClientProvider.Provide();

        await EnsureBaseAgentExists();

        return new AgentBase(BaseAgent!.AsBuilder().Use(runFunc: CustomAgentRunMiddleware, runStreamingFunc: null).Build(),
                             _serviceProvider.GetRequiredService<ISessionProvider>());
    }

    private async Task EnsureBaseAgentExists()
        => BaseAgent ??= await new ProductSearchWorkflow(_schemaProvider).ConstructProductSearchWorkflow(ChatClient!);

    private async Task<AIAgent> ConstructDefaultWorkflow(ChatClient chatClient)
    {
        var agentSchema = await _schemaProvider.Provide();

        var baseAgent = chatClient.AsIChatClient()
                                  .AsAIAgent(name: agentSchema.Name,
                                             instructions: "You are a friendly assistant.",
                                             tools: []);

        return baseAgent;
    }

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