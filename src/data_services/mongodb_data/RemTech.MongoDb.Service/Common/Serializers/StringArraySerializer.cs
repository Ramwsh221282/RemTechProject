using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTech.MongoDb.Service.Common.Abstractions.BsonSerialization;

namespace RemTech.MongoDb.Service.Common.Serializers;

public sealed class StringArraySerializer : CustomSerializer<string[]>
{
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        string[] value
    )
    {
        BsonArray array = [];
        foreach (string str in value)
            array.Add(str);
        BsonSerializer.Serialize(context.Writer, array);
    }

    public override string[] Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        BsonArray array = BsonArraySerializer.Instance.Deserialize(context, args);
        string[] strings = array.Select(i => i.AsString).ToArray();
        return strings;
    }
}
