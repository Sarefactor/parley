using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Schemas;

public class AgentSchema
{
    public AgentSchema(Guid agentSchemaId,
                       string name,
                       string instructions)
    {
        AgentSchemaId = agentSchemaId;
        Name = name;
        LastModified = DateTime.UtcNow;
        Instructions = instructions;
    }

    [JsonInclude]
    [JsonPropertyName("agentSchemaId")]
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid AgentSchemaId { get; private set; }

    [JsonInclude]
    [JsonPropertyName("name")]
    [BsonElement("name")]
    public string Name { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("lastUpdated")]
    [BsonElement("lastUpdated")]
    public DateTime LastModified { get; set; }

    [JsonInclude]
    [JsonPropertyName("instructions")]
    [BsonElement("instructions")]
    public string Instructions { get; private set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("workflowSchemas")]
    [BsonElement("workflowSchemas")]
    public List<WorkflowSchema> WorkflowSchemas { get; private set; } = new();

    public void SetWorkflowSchemas(List<WorkflowSchema> workflowSchemas)
        => WorkflowSchemas = workflowSchemas;
}