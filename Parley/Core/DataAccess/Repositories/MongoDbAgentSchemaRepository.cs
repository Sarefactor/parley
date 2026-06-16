using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Parley.Configuration.Options;
using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public class MongoDbAgentSchemaRepository : IAgentSchemaRepository
{
    private readonly ParleyConfig _config;

    private IMongoCollection<AgentSchema>? _collection;

    public MongoDbAgentSchemaRepository(IOptionsMonitor<ParleyConfig> config)
    {
        _config = config.CurrentValue;
    }

    public async Task<SearchResult<AgentSchema>> GetRange(int skip, int take)
    {
        EnsureCollectionExists();

        var filter = Builders<AgentSchema>.Filter.Empty;

        var totalResults = await _collection!.CountDocumentsAsync(filter);

        var results = await _collection.Find(filter)
                                       .Skip(skip)
                                       .Limit(take)
                                       .ToListAsync();

        return new SearchResult<AgentSchema>
        {
            TotalResults = (int)totalResults,
            Page = take > 0 ? (skip / take) + 1 : 1,
            PageSize = take,
            Results = results
        };
    }

    public async Task<AgentSchema?> Get(Guid agentSchemaId)
    {
        EnsureCollectionExists();

        var filter = Builders<AgentSchema>.Filter.Eq(x => x.AgentSchemaId, agentSchemaId);

        return (await _collection!.FindAsync(filter)).SingleOrDefault();
    }

    public async Task Delete(Guid agentSchemaId)
    {
        EnsureCollectionExists();

        var filter = Builders<AgentSchema>.Filter.Eq(x => x.AgentSchemaId, agentSchemaId);

        await _collection!.DeleteOneAsync(filter);
    }

    public void Upsert(AgentSchema agentSchema)
    {
        EnsureCollectionExists();

        var filter = Builders<AgentSchema>.Filter.Eq(x => x.AgentSchemaId, agentSchema.AgentSchemaId);

        var test = _collection!.Find(filter).SingleOrDefault();

        _collection!.ReplaceOne(filter,
                                agentSchema,
                                new ReplaceOptions { IsUpsert = true });
    }
    
    public void EnsureCollectionExists()
    {
        if (_collection != null)
            return;

        var client = new MongoClient(_config.Databases.MongoDb.ConnectionString);
        var database = client.GetDatabase(_config.Databases.MongoDb.Database);
        _collection =  database.GetCollection<AgentSchema>(_config.Databases.MongoDb.AgentSchemaCollection);
    }
}