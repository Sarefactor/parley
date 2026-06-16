using Microsoft.Extensions.Caching.Memory;
using Parley.Core.DataAccess.Models.Schemas;
using Parley.Core.DataAccess.Repositories;

namespace Parley.Providers;

internal class SchemaProvider : ISchemaProvider
{
    private readonly IAgentConfigurationRepository _agentConfigurationRepository;
    private readonly IAgentSchemaRepository _agentSchemaRepository;
    private readonly IMemoryCache _memoryCache;

    public SchemaProvider(IAgentConfigurationRepository agentConfigurationRepository,
                          IAgentSchemaRepository agentSchemaRepository,
                          IMemoryCache memoryCache)
    {
        _agentConfigurationRepository = agentConfigurationRepository;
        _agentSchemaRepository = agentSchemaRepository;
        _memoryCache = memoryCache;
    }

    public async Task<AgentSchema> Provide()
    {
        return (await _memoryCache.GetOrCreateAsync($"{nameof(SchemaProvider)}:{nameof(AgentSchema)}", async entry => await GetAgentSchema(), GetMemoryCacheOptions()))!;
    }

    private async Task<AgentSchema> GetAgentSchema()
    {
        var configuration = await GetAgentConfiguration();

        if (configuration == null || configuration.ActiveSchemaId == null)
            throw new Exception("Invalid configuration or active schema id.");

        var agentSchema = await _agentSchemaRepository.Get((Guid)configuration.ActiveSchemaId);

        if (agentSchema == null)
            throw new Exception("Could not get an agent schema from the db.");

        return agentSchema;
    }

    private async Task<AgentConfiguration> GetAgentConfiguration()
    {
        return (await _memoryCache.GetOrCreateAsync($"{nameof(SchemaProvider)}:{nameof(AgentConfiguration)}", async entry => await GetAgentConfigurationFromDb(), GetMemoryCacheOptions()))!;
    }

    private async Task<AgentConfiguration> GetAgentConfigurationFromDb()
    {
        var configuration = await _agentConfigurationRepository.Get();

        if (configuration == null)
            throw new Exception("No configuration found.");

        return configuration;
    }
    private MemoryCacheEntryOptions GetMemoryCacheOptions()
    {
        return new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = DateTimeOffset.UtcNow.AddHours(1).TimeOfDay
        };
    }

    public async Task SetActiveSchema(Guid agentSchemaId)
    {
        var agentSchema = await _agentSchemaRepository.Get(agentSchemaId);

        if (agentSchema == null)
            return;

        await _agentConfigurationRepository.UpdateActiveConversationId(agentSchemaId);
        _memoryCache.Set($"{nameof(SchemaProvider)}:{nameof(AgentSchema)}", agentSchema, GetMemoryCacheOptions());
    }
}