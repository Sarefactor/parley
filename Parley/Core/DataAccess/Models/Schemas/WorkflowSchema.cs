using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Variables;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Schemas;

public class WorkflowSchema
{
    public WorkflowSchema(Guid executionNodeId,
                          string name,
                          string intent,
                          string description)
    {
        Name = name;
        Intent = intent;
        Description = description;
        ExecutionNodeId = executionNodeId;
    }


    [JsonInclude]
    [JsonPropertyName("name")]
    [BsonElement("name")]
    public string Name { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("intent")]
    [BsonElement("intent")]
    public string Intent { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("description")]
    [BsonElement("description")]
    public string Description { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("lastUpdated")]
    [BsonElement("lastUpdated")]
    public DateTime LastModified { get; set; }

    [JsonInclude]
    [JsonPropertyName("executionNodeId")]
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ExecutionNodeId { get; private set; } = default!;

    [JsonInclude]
    [JsonPropertyName("nodes")]
    [BsonElement("nodes")]
    public Dictionary<Guid, NodeConfig> Nodes { get; set; } = new();

    [JsonInclude]
    [JsonPropertyName("workflowVariables")]
    [BsonElement("workflowVariables")]
    public List<WorkflowVariable> WorkflowVariables { get; private set; } = new();

    public List<WorkflowVariable> GetAllWorkflowVariables()
        => Nodes.SelectMany(x => x.Value.NodeVariables)
                .Concat(WorkflowVariables)
                .ToList();

    public void SetWorkflowVariables(List<WorkflowVariable> workflowVariables)
        => WorkflowVariables = workflowVariables;

    public void SetWorkflowNodes(Dictionary<Guid, NodeConfig> nodes)
        => Nodes = nodes;
}