using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed class AdvertisementQueryBuilderWithNoFilters
    : IQueryBuilder<AdvertisementQueryPayload, Advertisement>
{
    public FilterDefinition<Advertisement> Build() => Builders<Advertisement>.Filter.Empty;

    public void SetPayload(AdvertisementQueryPayload payload) { }
}
