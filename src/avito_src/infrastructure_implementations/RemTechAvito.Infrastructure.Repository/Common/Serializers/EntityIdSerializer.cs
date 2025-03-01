using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechCommon.Utils.Extensions;

namespace RemTechAvito.Infrastructure.Repository.Common.Serializers;

internal sealed class EntityIdSerializer : IBsonSerializer<TransportAdvertisementID>
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
        TransportAdvertisementID value
    )
    {
        var writer = context.Writer;
        writer.WriteGuid(value.Id);
    }

    public TransportAdvertisementID Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        var id = reader.ReadGuid();
        return new TransportAdvertisementID(GuidUtils.Existing(id));
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (TransportAdvertisementID)value);
    }

    public Type ValueType => typeof(TransportAdvertisementID);
}
