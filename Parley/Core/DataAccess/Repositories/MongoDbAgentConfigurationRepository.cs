using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Parley.Configuration.Options;
using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public class MongoDbAgentConfigurationRepository : IAgentConfigurationRepository
{
    private readonly ParleyConfig _config;
    private IMongoCollection<AgentConfiguration>? _collection;

    public MongoDbAgentConfigurationRepository(IOptionsMonitor<ParleyConfig> config)
    {
        _config = config.CurrentValue;
    }

    public async Task<AgentConfiguration?> Get()
    {
        EnsureCollectionExists();

        var filter = Builders<AgentConfiguration>.Filter.Eq(static x => x.ConfigurationId, AgentConfiguration.DefaultConfigurationId);
        return (await _collection!.FindAsync(filter)).SingleOrDefault();
    }

    public async Task UpdateActiveConversationId(Guid schemaId)
    {
        EnsureCollectionExists();

        var filter = Builders<AgentConfiguration>.Filter.Eq(static x => x.ConfigurationId, AgentConfiguration.DefaultConfigurationId);

        var configuration = (await _collection!.FindAsync(filter)).SingleOrDefault();

        if (configuration == null)
            return;

        configuration.SetActiveSchemaId(schemaId);

        _collection!.ReplaceOne(filter,
                                configuration,
                                new ReplaceOptions { IsUpsert = true });
    }

    public void EnsureCollectionExists()
    {
        if (_collection != null)
            return;

        var client = new MongoClient(_config.Databases.MongoDb.ConnectionString);
        var database = client.GetDatabase(_config.Databases.MongoDb.Database);
        _collection = database.GetCollection<AgentConfiguration>(_config.Databases.MongoDb.ConfigurationCollection);
    }
}