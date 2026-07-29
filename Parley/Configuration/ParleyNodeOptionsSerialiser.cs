using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using Parley.Core.DataAccess.Models.Nodes;
using Parley.Core.DataAccess.Models.Schemas;
using System.Text.Json;

namespace Parley.Configuration;

public sealed class ParleyNodeOptionsSerialiser : SerializerBase<JsonElement>
{
    public override void Serialize(BsonSerializationContext context,
                                   BsonSerializationArgs args,
                                   JsonElement value)
    {
        if (value is JsonElement element
            && element.ValueKind == JsonValueKind.Object)
        {
            var bson = (BsonValue)BsonDocument.Parse(element.GetRawText());
            BsonValueSerializer.Instance.Serialize(context, bson);
        }
        else
        {
            throw new Exception("Error serialising node options.");
        }
    }

    public override JsonElement Deserialize(BsonDeserializationContext context,
                                            BsonDeserializationArgs args)
    {
        var doc = BsonDocumentSerializer.Instance.Deserialize(context);

        var json = doc.ToJson(new JsonWriterSettings { OutputMode = JsonOutputMode.RelaxedExtendedJson });

        using var parsed = JsonDocument.Parse(json);
        return parsed.RootElement.Clone();
    }

    public static void ConfigureMongoDbSerialisation()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        BsonClassMap.RegisterClassMap<NodeConfig>(cm =>
        {
            cm.AutoMap();
            cm.GetMemberMap(c => c.Options).SetSerializer(new ParleyNodeOptionsSerialiser());
        });

        BsonClassMap.RegisterClassMap<WorkflowSchema>(cm =>
        {
            cm.AutoMap();
            cm.GetMemberMap(c => c.Nodes).SetSerializer(
                new DictionaryInterfaceImplementerSerializer<Dictionary<Guid, NodeConfig>>(
                    DictionaryRepresentation.ArrayOfDocuments,
                    new GuidSerializer(GuidRepresentation.Standard),
                    BsonSerializer.LookupSerializer<NodeConfig>()));
        });
    }
}