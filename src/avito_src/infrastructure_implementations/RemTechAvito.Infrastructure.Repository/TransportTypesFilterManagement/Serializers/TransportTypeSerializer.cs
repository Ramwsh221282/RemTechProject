using System.Data;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Serializers;

internal sealed class TransportTypeSerializer : IBsonSerializer<TransportType>
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

        writer.WriteName("type_implementor");
        writer.WriteString(value.Type);

        writer.WriteName("type_name");
        writer.WriteString(value.Name);

        writer.WriteName("type_link");
        writer.WriteString(value.Link);

        writer.WriteName("date_created");
        writer.WriteDateTime(value.CreatedOn.ToUnix());

        if (value.Type == TransportType.USER_TYPE)
        {
            UserTransportType userType = value.Unwrap<UserTransportType>();

            writer.WriteName("type_user_additions");
            writer.WriteStartArray();
            foreach (var addition in userType.Additions)
                writer.WriteString(addition);
            writer.WriteEndArray();

            writer.WriteName("type_user_profiles");
            writer.WriteStartArray();
            foreach (var profile in userType.Profiles)
                writer.WriteString(profile);
            writer.WriteEndArray();
        }

        writer.WriteEndDocument();
    }

    public TransportType Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        var implementor = string.Empty;
        var name = string.Empty;
        var link = string.Empty;
        DateOnly createdOn = default;
        List<string> textSearchAdditions = [];
        List<string> profileAdditions = [];

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var property = reader.ReadName();
            switch (property)
            {
                case "type_implementor":
                    implementor = reader.ReadString();
                    break;
                case "type_name":
                    name = reader.ReadString();
                    break;
                case "type_link":
                    link = reader.ReadString();
                    break;
                case "date_created":
                    createdOn = reader.ReadDateTime().FromUnix();
                    break;
                case "type_user_additions":
                    reader.ReadStartArray();
                    while (reader.ReadBsonType() != BsonType.EndOfDocument)
                        textSearchAdditions.Add(reader.ReadString());
                    reader.ReadEndArray();
                    break;
                case "type_user_profiles":
                    reader.ReadStartArray();
                    while (reader.ReadBsonType() != BsonType.EndOfDocument)
                        profileAdditions.Add(reader.ReadString());
                    reader.ReadEndArray();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndDocument();
        return implementor switch
        {
            TransportType.USER_TYPE => UserTransportType
                .Create(name, link, createdOn, textSearchAdditions)
                .Value.Unwrap<UserTransportType>()
                .Value.Update(profileAdditions, textSearchAdditions),
            TransportType.SYSTEM_TYPE => SystemTransportType.Create(name, link, createdOn),
            _ => throw new InvalidConstraintException("Unknown transport type"),
        };
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
