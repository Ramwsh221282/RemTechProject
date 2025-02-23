using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileLinkIdSerializer : IBsonSerializer<ParserProfileLinkId>
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
        ParserProfileLinkId value
    )
    {
        var writer = context.Writer;
        writer.WriteGuid(value.Id);
    }

    public ParserProfileLinkId Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        Guid id = reader.ReadGuid();
        return new ParserProfileLinkId(id);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (ParserProfileLinkId)value);
    }

    public Type ValueType => typeof(ParserProfileLinkId);
}
