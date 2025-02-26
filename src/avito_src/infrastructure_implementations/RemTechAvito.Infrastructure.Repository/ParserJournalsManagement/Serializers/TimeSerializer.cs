using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.Common.ValueObjects;

namespace RemTechAvito.Infrastructure.Repository.ParserJournalsManagement.Serializers;

internal sealed class TimeSerializer : IBsonSerializer<Time>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        return Deserialize(context, args);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Time value)
    {
        var writer = context.Writer;
        writer.WriteStartDocument();
        writer.WriteName("time_hours");
        writer.WriteInt32(value.Hours);
        writer.WriteName("time_minutes");
        writer.WriteInt32(value.Minutes);
        writer.WriteName("time_seconds");
        writer.WriteInt32(value.Seconds);
        writer.WriteEndDocument();
    }

    public Time Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var hours = 0;
        var minutes = 0;
        var seconds = 0;
        var reader = context.Reader;
        reader.ReadStartDocument();
        while (reader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var name = reader.ReadName();
            switch (name)
            {
                case "time_hours":
                    hours = reader.ReadInt32();
                    break;
                case "time_minutes":
                    reader.ReadInt32();
                    break;
                case "time_seconds":
                    seconds = reader.ReadInt32();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }

        reader.ReadEndDocument();
        return Time.Create(hours, minutes, seconds);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (Time)value);
    }

    public Type ValueType => typeof(Time);
}
