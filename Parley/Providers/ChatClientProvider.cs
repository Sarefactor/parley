using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using Parley.Configuration.Options;

namespace Parley.Providers;

internal class ChatClientProvider : IChatClientProvider
{
    private readonly ParleyConfig _config;

    public ChatClientProvider(IOptionsMonitor<ParleyConfig> config)
    {
        _config = config.CurrentValue;
    }

    public IChatClient Provide()
    {
        return _config.AgentProvider switch
        {
            Configuration.Enums.AgentProviderType.OpenAi => CreateOpenAiChatClient(),
            _ => throw new NotImplementedException($"Unable to create {nameof(ChatClient)} for provider: {_config.AgentProvider.ToString()}.")
        };
    }

    private IChatClient CreateOpenAiChatClient()
        => new OpenAIClient(_config.OpenAiConfig.ApiKey).GetChatClient(_config.OpenAiConfig.Model).AsIChatClient();
}