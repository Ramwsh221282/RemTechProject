using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class MongoPriceFilterQuery
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

        var builder = Builders<TransportAdvertisement>.Filter;
        var filter = price.Predicate switch
        {
            "LESS" => builder.Lt(ad => ad.Price.Value, price.Price.Value),
            "MORE" => builder.Gt(ad => ad.Price.Value, price.Price.Value),
            "EQUAL" => builder.Eq(ad => ad.Price.Value, price.Price.Value),
            _ => null,
        };

        if (filter != null)
            filters.Add(filter);
    }
}
