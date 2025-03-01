using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileNameSerializer : IBsonSerializer<ParserProfileName>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        return Deserialize(context, args);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        ParserProfileName value
    )
    {
        var writer = context.Writer;
        writer.WriteString(value.Name);
    }

    public ParserProfileName Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var name = context.Reader.ReadString();
        return ParserProfileName.Create(name);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (ParserProfileName)value);
    }

    public Type ValueType => typeof(ParserProfileName);
}
