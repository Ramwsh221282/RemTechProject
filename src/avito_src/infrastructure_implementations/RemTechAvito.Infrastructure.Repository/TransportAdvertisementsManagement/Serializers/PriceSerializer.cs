using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

public sealed class PriceSerializer : IBsonSerializer<Price>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Price value)
    {
        var writer = context.Writer;
        writer.WriteStartDocument();

        writer.WriteName("price_value");
        writer.WriteInt64(value.Value);

        writer.WriteName("price_currency");
        writer.WriteString(value.Currency);

        writer.WriteName("price_extra");
        writer.WriteString(value.Extra);

        writer.WriteEndDocument();
    }

    public Price Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        uint value = 0;
        string currency = string.Empty;
        string extra = string.Empty;

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var elementName = reader.ReadName();
            switch (elementName)
            {
                case "price_value":
                    value = (uint)reader.ReadInt64();
                    break;
                case "price_currency":
                    currency = reader.ReadString();
                    break;
                case "price_extra":
                    extra = reader.ReadString();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        return Price.Create(value, currency, extra);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (Price)value);
    }

    public Type ValueType => typeof(Price);
}
