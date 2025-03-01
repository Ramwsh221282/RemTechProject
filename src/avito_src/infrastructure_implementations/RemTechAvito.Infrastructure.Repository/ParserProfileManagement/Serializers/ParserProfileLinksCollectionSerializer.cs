using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserProfileManagement.Serializers;

internal sealed class ParserProfileLinksCollectionSerializer
    : IBsonSerializer<IReadOnlyCollection<ParserProfileLink>>
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
        IReadOnlyCollection<ParserProfileLink> value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();
        writer.WriteName("profile_links");
        writer.WriteStartArray();

        foreach (var link in value)
        {
            writer.WriteStartDocument();

            writer.WriteName("link_name");
            writer.WriteString(link.Name);

            writer.WriteName("link_value");
            writer.WriteString(link.Link);

            var unwrapped = link.Unwrap<ParserProfileLinkWithAdditions>();
            if (!unwrapped.IsSuccess)
            {
                writer.WriteEndDocument();
                continue;
            }

            writer.WriteName("link_additions");
            writer.WriteStartArray();

            foreach (var addition in unwrapped.Value.Additions)
                writer.WriteString(addition);
            writer.WriteEndArray();

            writer.WriteEndDocument();
        }

        writer.WriteEndArray();
        writer.WriteEndDocument();
    }

    public IReadOnlyCollection<ParserProfileLink> Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var list = new List<ParserProfileLink>();
        var reader = context.Reader;
        reader.ReadStartDocument();
        reader.ReadStartArray();
        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            reader.ReadStartDocument();
            List<string> additions = [];
            var name = string.Empty;
            var value = string.Empty;
            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var property = reader.ReadName();
                switch (property)
                {
                    case "link_name":
                        name = reader.ReadString();
                        break;
                    case "link_value":
                        value = reader.ReadString();
                        break;
                    case "link_additions":
                        reader.ReadStartArray();
                        while (reader.ReadBsonType() != BsonType.EndOfDocument)
                            additions.Add(reader.ReadString());
                        reader.ReadEndArray();
                        break;
                    default:
                        reader.SkipValue();
                        break;
                }
            }

            list.Add(ParserProfileLinkFactory.Create(name, value, additions));
            reader.ReadEndDocument();
        }

        reader.ReadEndArray();
        reader.ReadEndDocument();
        return list;
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (IReadOnlyCollection<ParserProfileLink>)value);
    }

    public Type ValueType => typeof(IReadOnlyCollection<ParserProfileLink>);
}
