using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

public sealed class AdvertisementUrlSerializer : IBsonSerializer<AdvertisementUrl>
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
        AdvertisementUrl value
    )
    {
        var writer = context.Writer;
        writer.WriteString(value.Url);
    }

    public AdvertisementUrl Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        var url = reader.ReadString();
        return AdvertisementUrl.Create(url);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (AdvertisementUrl)value);
    }

    public Type ValueType => typeof(AdvertisementUrl);
}
