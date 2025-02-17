using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Serializers;

public sealed class TransportTypeSerializer : IBsonSerializer<TransportType>
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
        TransportType value
    )
    {
        var writer = context.Writer;

        writer.WriteStartDocument();

        writer.WriteName("type_name");
        writer.WriteString(value.Name);

        writer.WriteName("type_link");
        writer.WriteString(value.Link);

        writer.WriteName("date_created");
        writer.WriteDateTime(value.CreatedOn.ToUnix());

        writer.WriteEndDocument();
    }

    public TransportType Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        string name = string.Empty;
        string link = string.Empty;
        DateOnly date = DateOnly.MinValue;

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            string property = reader.ReadName();
            switch (property)
            {
                case "type_name":
                    name = reader.ReadString();
                    break;
                case "type_link":
                    link = reader.ReadString();
                    break;
                case "date_created":
                    date = reader.ReadDateTime().FromUnix();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndDocument();
        return TransportType.Create(name, link, date);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (TransportType)value);
    }

    public Type ValueType => typeof(TransportType);
}
