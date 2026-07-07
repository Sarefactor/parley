using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Parley.Configuration.Options;
using Parley.Core.DataAccess.Models.Schemas;

namespace Parley.Core.DataAccess.Repositories;

public class WorkflowSchemaRepository : IWorkflowSchemaRepository
{
    private IMongoCollection<WorkflowSchema>? _collection;
    private readonly ParleyConfig _config;

    public WorkflowSchemaRepository(IOptionsMonitor<ParleyConfig> config)
    {
        _config = config.CurrentValue;
    }

    public async Task<SearchResult<WorkflowSchema>> GetRange(int skip, int take)
    {
        EnsureCollectionExists();

        var filter = Builders<WorkflowSchema>.Filter.Empty;

        var totalResults = await _collection!.CountDocumentsAsync(filter);

        var results = await _collection.Find(filter)
                                       .Skip(skip)
                                       .Limit(take)
                                       .ToListAsync();

        return new SearchResult<WorkflowSchema>
        {
            TotalResults = (int)totalResults,
            Page = take > 0 ? (skip / take) + 1 : 1,
            PageSize = take,
            Results = results
        };
    }

    public async Task<WorkflowSchema?> Get(Guid workflowSchemaId)
    {
        EnsureCollectionExists();

        var filter = Builders<WorkflowSchema>.Filter.Eq(x => x.ExecutionNodeId, workflowSchemaId);

        return (await _collection!.FindAsync(filter)).SingleOrDefault();
    }

    public async Task Delete(Guid agentSchemaId)
    {
        EnsureCollectionExists();

        var filter = Builders<WorkflowSchema>.Filter.Eq(x => x.ExecutionNodeId, agentSchemaId);

        await _collection!.DeleteOneAsync(filter);
    }

    public void Upsert(WorkflowSchema workflowSchema)
    {
        EnsureCollectionExists();

        var filter = Builders<WorkflowSchema>.Filter.Eq(x => x.ExecutionNodeId, workflowSchema.ExecutionNodeId);

        var test = _collection!.Find(filter).SingleOrDefault();

        _collection!.ReplaceOne(filter,
                                workflowSchema,
                                new ReplaceOptions { IsUpsert = true });
    }

    private void EnsureCollectionExists()
    {
        if (_collection != null)
            return;

        var client = new MongoClient(_config.Databases.MongoDb.ConnectionString);
        var database = client.GetDatabase(_config.Databases.MongoDb.Database);
        _collection = database.GetCollection<WorkflowSchema>(_config.Databases.MongoDb.WorkflowSchemaCollection);
    }
}
