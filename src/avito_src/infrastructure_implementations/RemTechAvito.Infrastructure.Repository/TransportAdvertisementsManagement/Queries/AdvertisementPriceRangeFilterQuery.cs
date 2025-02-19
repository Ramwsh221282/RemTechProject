using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

public sealed class AdvertisementPriceRangeFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        PriceRangeDto? price = dto.PriceRange;
        if (price == null)
            return;

        if (price.ValueMin == 0)
            return;

        if (price.ValueMax == 0)
            return;

        if (price.ValueMax < price.ValueMin)
            return;

        var filter = new BsonDocument(
            "Price.price_value",
            new BsonDocument() { { "$gte", price.ValueMin }, { "$lte", price.ValueMax } }
        );

        filters.Add(filter);
    }
}
