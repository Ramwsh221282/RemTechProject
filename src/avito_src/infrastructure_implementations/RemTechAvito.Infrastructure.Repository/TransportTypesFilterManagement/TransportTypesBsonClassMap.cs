using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement;

internal sealed class TransportTypesBsonClassMap : BsonClassMap<TransportType>
{
    public TransportTypesBsonClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapField(t => t.Link);
        MapField(t => t.Name);
        MapField(t => t.CreatedOn).SetSerializer(new DateOnlySerializer());
    }
}
