using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileStateSerializer : IBsonSerializer<ParserProfileState>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    ) => Deserialize(context, args);

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        ParserProfileState value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();
        writer.WriteName("state_value");
        writer.WriteBoolean(value.IsActive);
        writer.WriteName("state_description");
        writer.WriteString(value.Description);
        writer.WriteEndDocument();
    }

    public ParserProfileState Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        bool isActive = false;
        var reader = context.Reader;
        reader.ReadStartDocument();
        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            string name = reader.ReadName();
            switch (name)
            {
                case "state_value":
                    isActive = reader.ReadBoolean();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }
        reader.ReadEndDocument();

        return isActive ? ParserProfileState.CreateActive() : ParserProfileState.CreateInactive();
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    ) => Serialize(context, args, (ParserProfileState)value);

    public Type ValueType => typeof(ParserProfileState);
}
