using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileNameSerializer : IBsonSerializer<ParserProfileName>
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
        ParserProfileName value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();
        writer.WriteName("profile_name");
        writer.WriteString(value.Name);
        writer.WriteEndDocument();
    }

    public ParserProfileName Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartDocument();
        var value = string.Empty;

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var name = reader.ReadName();
            if (name == "profile_name")
                value = reader.ReadString();
            reader.SkipValue();
        }

        reader.ReadEndDocument();
        return ParserProfileName.Create(value);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (ParserProfileName)value);
    }

    public Type ValueType => typeof(ParserProfileName);
}
