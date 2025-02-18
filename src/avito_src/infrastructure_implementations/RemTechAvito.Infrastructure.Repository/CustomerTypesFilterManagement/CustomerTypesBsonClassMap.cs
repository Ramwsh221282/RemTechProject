using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;

namespace RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement;

internal sealed class CustomerTypesBsonClassMap : BsonClassMap<CustomerType>
{
    public CustomerTypesBsonClassMap()
    {
        AutoMap();
        SetIgnoreExtraElements(true);
        MapField(t => t.Type);
        MapField(t => t.CreatedOn).SetSerializer(new DateOnlySerializer());
    }
}
