using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

public sealed class PhotoAttachmentsSerializer : IBsonSerializer<PhotoAttachments>
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
        PhotoAttachments value
    )
    {
        var writer = context.Writer;
        writer.WriteStartArray();

        foreach (var path in value.Photos)
        {
            writer.WriteString(path.Path);
        }

        writer.WriteEndArray();
    }

    public PhotoAttachments Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartArray();

        List<Photo> photos = [];
        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var path = reader.ReadString();
            Photo photo = Photo.Create(path);
            photos.Add(photo);
        }

        reader.ReadEndArray();
        return new PhotoAttachments(photos);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (PhotoAttachments)value);
    }

    public Type ValueType => typeof(PhotoAttachments);
}
