using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTechAvito.Core.FiltersManagement.CustomerStates;

namespace RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement;

internal sealed class CustomerStatesBsonClassMap : BsonClassMap<CustomerState>
{
    public CustomerStatesBsonClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapField(state => state.State);
        MapField(state => state.CreatedOn).SetSerializer(new DateOnlySerializer());
    }
}
