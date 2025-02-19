using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.Common.Serializers;

internal sealed class EntityIdSerializer : IBsonSerializer<EntityID>
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
        EntityID value
    )
    {
        var writer = context.Writer;
        writer.WriteGuid(value.Id);
    }

    public EntityID Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        Guid id = reader.ReadGuid();
        return EntityID.Existing(id);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (EntityID)value);
    }

    public Type ValueType => typeof(EntityID);
}
