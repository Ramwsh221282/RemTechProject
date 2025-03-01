using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileIdSerializer : IBsonSerializer<ParserProfileId>
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
        ParserProfileId value
    )
    {
        var writer = context.Writer;
        writer.WriteGuid(value.Id);
    }

    public ParserProfileId Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        return new ParserProfileId(reader.ReadGuid());
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (ParserProfileId)value);
    }

    public Type ValueType => typeof(ParserProfileId);
}
