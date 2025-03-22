using MongoDB.Bson.Serialization;

namespace RemTech.MongoDb.Service.Common.Abstractions.BsonClassMapping;

public abstract class CustomClassMap<TClass> : BsonClassMap<TClass>
{
    public void Register() => BsonClassMap.RegisterClassMap(this);
}
