using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTech.MongoDb.Service.Common.Abstractions.BsonSerialization;

namespace RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.BsonSerializers;

public sealed class AdvertisementCharacteristicsSerializer
    : CustomSerializer<AdvertisementCharacteristic[]>
{
    public override void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        AdvertisementCharacteristic[] value
    )
    {
        BsonArray array = [];
        foreach (var characteristic in value)
        {
            BsonDocument document = new BsonDocument
            {
                { "name", characteristic.Name },
                { "value", characteristic.Value },
            };
            array.Add(document);
        }
        BsonSerializer.Serialize(context.Writer, array);
    }

    public override AdvertisementCharacteristic[] Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        List<AdvertisementCharacteristic> ctxObjects = [];
        BsonArray array = BsonArraySerializer.Instance.Deserialize(context, args);
        foreach (var item in array)
        {
            BsonDocument @object = (BsonDocument)item;
            ctxObjects.Add(new(@object["name"].AsString, @object["value"].AsString));
        }
        return ctxObjects.ToArray();
    }
}
