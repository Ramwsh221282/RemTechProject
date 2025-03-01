using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

internal sealed class OwnerInformationSerializer : IBsonSerializer<OwnerInformation>
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
        OwnerInformation value
    )
    {
        var writer = context.Writer;

        writer.WriteStartDocument();

        writer.WriteName("owner_description");
        writer.WriteString(value.Text);

        writer.WriteName("owner_status");
        writer.WriteString(value.Status);

        writer.WriteEndDocument();
    }

    public OwnerInformation Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        var text = string.Empty;
        var status = string.Empty;

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var elementName = reader.ReadName();
            switch (elementName)
            {
                case "owner_description":
                    text = reader.ReadString();
                    break;
                case "owner_status":
                    status = reader.ReadString();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndDocument();
        return OwnerInformation.Create(text, status);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (OwnerInformation)value);
    }

    public Type ValueType => typeof(OwnerInformation);
}
