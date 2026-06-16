using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Parley.Core.DataAccess.Models.Schemas;

public class AgentConfiguration
{
    public static readonly string DefaultConfigurationId = "waterworksBot:configuration";

    public AgentConfiguration() {}

    [BsonId]
    public ObjectId Id { get; private set; }

    [JsonInclude]
    [JsonPropertyName("configurationId")]
    [BsonElement("configurationId")]
    public string ConfigurationId { get; private set; } = DefaultConfigurationId;

    [JsonInclude]
    [JsonPropertyName("activeSchemaId")]
    [BsonElement("activeSchemaId")]
    public Guid? ActiveSchemaId { get; private set; } = default!;

    public void SetActiveSchemaId(Guid activeSchemaId)
        => ActiveSchemaId = activeSchemaId;
}