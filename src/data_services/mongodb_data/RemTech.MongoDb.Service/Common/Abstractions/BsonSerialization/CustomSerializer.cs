using MongoDB.Bson.Serialization;

namespace RemTech.MongoDb.Service.Common.Abstractions.BsonSerialization;

public abstract class CustomSerializer<TTarget> : IBsonSerializer<TTarget>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    ) => Deserialize(context, args)!;

    public abstract void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        TTarget value
    );

    public abstract TTarget Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    );

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    ) => Serialize(context, args, (TTarget)value);

    public void Register() => BsonSerializer.RegisterSerializer(this);

    public Type ValueType => typeof(TTarget);
}
