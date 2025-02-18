using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class MongoDateFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        DateFilterDto? date = dto.Date;
        if (date == null)
            return;

        var builder = Builders<TransportAdvertisement>.Filter;
        var filter = date.Predicate switch
        {
            "LESS" => builder.Lt(ad => ad.CreatedOn.ToUnix(), date.Date.ToUnix()),
            "MORE" => builder.Gt(ad => ad.CreatedOn.ToUnix(), date.Date.ToUnix()),
            "EQUAL" => builder.Eq(ad => ad.CreatedOn.ToUnix(), date.Date.ToUnix()),
            _ => null,
        };

        if (filter != null)
            filters.Add(filter);
    }
}
