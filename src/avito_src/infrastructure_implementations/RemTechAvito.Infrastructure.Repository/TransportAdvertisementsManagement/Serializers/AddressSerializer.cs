using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

internal sealed class AddressSerializer : IBsonSerializer<Address>
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
        Address value
    )
    {
        var writer = context.Writer;
        writer.WriteString(value.Text);
    }

    public Address Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        var text = reader.ReadString();
        return Address.Create(text);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (Address)value);
    }

    public Type ValueType => typeof(Address);
}
