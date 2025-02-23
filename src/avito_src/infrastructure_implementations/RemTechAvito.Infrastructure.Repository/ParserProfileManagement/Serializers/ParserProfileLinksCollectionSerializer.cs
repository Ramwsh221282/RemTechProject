using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks;
using RemTechAvito.Core.ParserProfileManagement.Entities.ParserProfileLinks.ValueObjects;
using RemTechCommon.Utils.Extensions;

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
        writer.WriteStartArray();
        foreach (var link in value)
        {
            writer.WriteStartDocument();

            writer.WriteName("link_id");
            writer.WriteGuid(link.Id.Id);

            writer.WriteName("link_body");
            writer.WriteStartDocument();
            writer.WriteName("link_link");
            writer.WriteString(link.Body.Link);
            writer.WriteName("link_mark");
            writer.WriteString(link.Body.Mark);
            writer.WriteEndDocument();

            writer.WriteEndDocument();
        }
        writer.WriteEndArray();
    }

    public IReadOnlyCollection<ParserProfileLink> Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var list = new List<ParserProfileLink>();
        var reader = context.Reader;
        reader.ReadStartArray();

        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            reader.ReadStartDocument();

            Guid id = Guid.Empty;
            string mark = string.Empty;
            string link = string.Empty;

            while (reader.ReadBsonType() != BsonType.EndOfDocument)
            {
                id = reader.ReadGuid();
                reader.ReadStartDocument();
                while (reader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    reader.ReadName("link_link");
                    link = reader.ReadString();
                    reader.ReadName("link_mark");
                    mark = reader.ReadString();
                }
                reader.ReadEndDocument();
            }

            reader.ReadEndDocument();

            list.Add(
                new ParserProfileLink(
                    ParserProfileLinkBody.Create(mark, link),
                    new ParserProfileLinkId(GuidUtils.Existing(id))
                )
            );
        }

        reader.ReadEndArray();
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
