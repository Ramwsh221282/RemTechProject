using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

public class AdvertisementIDSerializer : IBsonSerializer<AdvertisementID>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    ) => Deserialize(context, args);

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        AdvertisementID value
    )
    {
        var writer = context.Writer;
        writer.WriteInt64(value.Id);
    }

    public AdvertisementID Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        long id = reader.ReadInt64();
        return AdvertisementID.Create(id);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    ) => Serialize(context, args, (AdvertisementID)value);

    public Type ValueType => typeof(AdvertisementID);
}
