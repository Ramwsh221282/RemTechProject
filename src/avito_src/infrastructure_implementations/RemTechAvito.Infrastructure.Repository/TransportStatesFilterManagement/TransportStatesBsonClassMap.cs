using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTechAvito.Core.FiltersManagement.TransportStates;

namespace RemTechAvito.Infrastructure.Repository.TransportStatesFilterManagement;

internal sealed class TransportStatesBsonClassMap : BsonClassMap<TransportState>
{
    public TransportStatesBsonClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapField(s => s.State);
        MapField(s => s.DateCreated).SetSerializer(new DateOnlySerializer());
    }
}
