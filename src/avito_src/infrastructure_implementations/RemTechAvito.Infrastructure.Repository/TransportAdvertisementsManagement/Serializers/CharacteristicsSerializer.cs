using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

internal sealed class CharacteristicsSerializer : IBsonSerializer<Characteristics>
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
        Characteristics value
    )
    {
        var writer = context.Writer;
        writer.WriteStartArray();

        foreach (var characteristic in value.Data)
        {
            writer.WriteStartDocument();

            writer.WriteName("attribute_name");
            writer.WriteString(characteristic.Name);

            writer.WriteName("attribute_value");
            writer.WriteString(characteristic.Value);

            writer.WriteEndDocument();
        }

        writer.WriteEndArray();
    }

    public Characteristics Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartArray();

        var characteristicsList = new List<Characteristic>();

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            reader.ReadStartDocument();

            string name = string.Empty;
            string value = string.Empty;

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var propertyName = reader.ReadName();

                switch (propertyName)
                {
                    case "attribute_name":
                        name = reader.ReadString();
                        break;
                    case "attribute_value":
                        value = reader.ReadString();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            Characteristic ctx = Characteristic.Create(name, value);
            characteristicsList.Add(ctx);

            reader.ReadEndDocument();
        }

        reader.ReadEndArray();
        return new Characteristics(characteristicsList);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (Characteristics)value);
    }

    public Type ValueType => typeof(Characteristics);
}
