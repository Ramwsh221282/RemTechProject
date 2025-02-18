using MongoDB.Bson.Serialization;
using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

internal sealed class DescriptionSerializer : IBsonSerializer<Description>
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
        Description value
    )
    {
        var writer = context.Writer;
        writer.WriteString(value.Text);
    }

    public Description Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        string text = reader.ReadString();
        return Description.Create(text);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (Description)value);
    }

    public Type ValueType => typeof(Description);
}
