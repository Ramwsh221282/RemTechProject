using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class AdvertisementPriceFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        PriceFilterDto? price = dto.Price;
        if (price == null)
            return;

        if (price.Price.Value > 0)
        {
            var filter = price.Predicate switch
            {
                "LESS" => new BsonDocument(
                    "Price.price_value",
                    new BsonDocument() { { "$lte", price.Price.Value } }
                ),
                "MORE" => new BsonDocument(
                    "Price.price_value",
                    new BsonDocument() { { "$gte", price.Price.Value } }
                ),
                "EQUAL" => new BsonDocument(
                    "Price.price_value",
                    new BsonDocument() { { "$eq", price.Price.Value } }
                ),
                _ => null,
            };
            if (filter != null)
                filters.Add(filter);
        }

        if (!string.IsNullOrWhiteSpace(price.Price.Extra))
        {
            var regExp = price.Price.Extra;
            var filter = new BsonDocument(
                "Price.price_extra",
                new BsonDocument() { { "$regex", regExp }, { "$options", "i" } }
            );
            filters.Add(filter);
        }
    }
}
