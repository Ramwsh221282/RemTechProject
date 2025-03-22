using MongoDB.Bson.Serialization.Serializers;
using RemTech.MongoDb.Service.Common.Abstractions.BsonClassMapping;
using RemTech.MongoDb.Service.Common.Serializers;
using DateTimeSerializer = MongoDB.Bson.Serialization.Serializers.DateTimeSerializer;

namespace RemTech.MongoDb.Service.Common.Models.ParsersManagement.BsonClassMaps;

public class ParserBsonClassMap : CustomClassMap<Parser>
{
    public ParserBsonClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapProperty(p => p.ServiceName).SetSerializer(new StringSerializer());
        MapProperty(p => p.State).SetSerializer(new StringSerializer());
        MapProperty(p => p.Links).SetSerializer(new StringArraySerializer());
        MapProperty(p => p.LastRun).SetSerializer(new DateTimeSerializer());
        MapProperty(p => p.NextRun).SetSerializer(new DateTimeSerializer());
        MapProperty(p => p.RepeatEveryHours).SetSerializer(new Int32Serializer());
    }
}
