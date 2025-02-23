using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileLinkBodySerializer : IBsonSerializer<ParserProfileLinkBody>
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
        ParserProfileLinkBody value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();
        writer.WriteName("body_mark");
        writer.WriteString(value.Mark);
        writer.WriteName("body_link");
        writer.WriteString(value.Link);
        writer.WriteEndDocument();
    }

    public ParserProfileLinkBody Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        string mark = string.Empty;
        string link = string.Empty;

        var reader = context.Reader;
        reader.ReadStartDocument();

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            string name = reader.ReadName();
            switch (name)
            {
                case "body_mark":
                    mark = reader.ReadString();
                    break;
                case "body_link":
                    link = reader.ReadString();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }
        reader.ReadEndDocument();
        return ParserProfileLinkBody.Create(mark, link);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (ParserProfileLinkBody)value);
    }

    public Type ValueType => typeof(ParserProfileLinkBody);
}
