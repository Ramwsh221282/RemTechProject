using MongoDB.Bson.Serialization;
using RemTech.MongoDb.Service.Common.Abstractions.BsonSerialization;
using RemTechCommon.Utils.Converters;

namespace RemTech.MongoDb.Service.Common.Serializers;

public sealed class DateTimeSerializer : CustomSerializer<DateTime>
{
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        DateTime value
    )
    {
        long unixDate = value.ToUnixFromDateTime();
        context.Writer.WriteDateTime(unixDate);
    }

    public override DateTime Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    ) => context.Reader.ReadDateTime().FromUnixToDateTime();
}
