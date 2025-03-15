using MongoDB.Bson.Serialization;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.Common.Serializers;

internal sealed class DateOnlySerializer : IBsonSerializer<DateOnly>
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
        DateOnly value
    )
    {
        var writer = context.Writer;
        writer.WriteDateTime(value.ToUnixFromDateOnly());
    }

    public DateOnly Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        return reader.ReadDateTime().FromUnixToDateOnly();
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (DateOnly)value);
    }

    public Type ValueType => typeof(DateOnly);
}
