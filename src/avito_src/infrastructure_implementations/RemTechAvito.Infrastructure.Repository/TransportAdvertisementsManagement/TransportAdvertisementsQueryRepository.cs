using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement;

internal sealed class TransportAdvertisementsQueryRepository(
    MongoClient client,
    TransportAdvertisementsQueryResolver resolver
) : ITransportAdvertisementsQueryRepository
{
    public async Task<AdvertisementItemResponse[]> Query(
        GetAnalyticsItemsRequest query,
        CancellationToken ct = default
    )
    {
        string? sortOrder = query.SortOrder;
        SortDefinition<TransportAdvertisement>? sortDefinition = sortOrder switch
        {
            "ASC" => Builders<TransportAdvertisement>.Sort.Ascending("Price.price_value"),
            "DESC" => Builders<TransportAdvertisement>.Sort.Descending("Price.price_value"),
            _ => null,
        };

        var filter = resolver.Resolve(query.FilterData);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportAdvertisement>(
            TransportAdvertisementsRepository.CollectionName
        );

        IFindFluent<TransportAdvertisement, TransportAdvertisement> findQuery = collection.Find(
            filter
        );

        if (sortDefinition != null)
            findQuery.Sort(sortDefinition);

        var paginatedAds = await findQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Limit(query.PageSize)
            .ToListAsync(ct);

        AdvertisementItemResponse[] responses = Array.Empty<AdvertisementItemResponse>();

        if (paginatedAds != null)
            responses = paginatedAds.Select(ad => ad.ToResponse()).ToArray();

        return responses;
    }

    public async Task<AnalyticsStatsResponse> Query(
        GetAnalyticsStatsRequest query,
        CancellationToken ct = default
    )
    {
        var filter = resolver.Resolve(query.FilterData);
        var db = client.GetDatabase(TransportAdvertisementsRepository.DbName);
        var collection = db.GetCollection<TransportAdvertisement>(
            TransportAdvertisementsRepository.CollectionName
        );

        var facet = AggregateFacet.Create(
            "aggregates",
            PipelineDefinition<TransportAdvertisement, BsonDocument>.Create(
                new[]
                {
                    new BsonDocument(
                        "$group",
                        new BsonDocument
                        {
                            { "_id", BsonNull.Value },
                            { "totalCount", new BsonDocument("$sum", 1) },
                            { "avgPrice", new BsonDocument("$avg", "$Price.price_value") },
                            { "minPrice", new BsonDocument("$min", "$Price.price_value") },
                            { "maxPrice", new BsonDocument("$max", "$Price.price_value") },
                        }
                    ),
                }
            )
        );

        var aggregation = await collection.Aggregate().Match(filter).Facet(facet).ToListAsync(ct);
        if (aggregation == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        var facetResults = aggregation.FirstOrDefault();
        if (facetResults == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        var facets = facetResults.Facets;
        if (facets == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        var aggregates = facets.First();
        if (aggregates == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        var documents = aggregates.Output<BsonDocument>();
        if (documents == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        if (!documents.Any())
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        var aggregateDocumentResults = documents.First();
        if (aggregateDocumentResults == null)
            return new AnalyticsStatsResponse(0, 0, 0, 0);

        int totalCount = aggregateDocumentResults["totalCount"].ToInt32();
        double avgPrice = aggregateDocumentResults["avgPrice"].ToDouble();
        long maxPrice = aggregateDocumentResults["maxPrice"].ToInt64();
        long minPrice = aggregateDocumentResults["minPrice"].ToInt64();

        return new AnalyticsStatsResponse(totalCount, avgPrice, maxPrice, minPrice);
    }
}

internal static class TransportAdvertisementConvertExtensions
{
    internal static AdvertisementItemResponse ToResponse(this TransportAdvertisement advertisement)
    {
        AdvertisementItemResponse response = new AdvertisementItemResponse();
        response.Id = advertisement.EntityId.Id;
        response.AdvertisementId = advertisement.AdvertisementId.Id;
        response.Title = advertisement.Title.Text;
        response.Description = advertisement.Description.Text;
        response.ImageLinks = advertisement.PhotoAttachments.Photos.Select(p => p.Path).ToArray();
        response.Characteristics = advertisement
            .Characteristics.Data.Select(d => new CharacteristicsResponse()
            {
                Name = d.Name,
                Value = d.Value,
            })
            .ToArray();
        response.Owner.Status = advertisement.OwnerInformation.Status;
        response.Owner.Description = advertisement.OwnerInformation.Text;
        response.SourceUrl = advertisement.Url.Url;
        response.Address = advertisement.Address.Text;
        response.Price.Currency = advertisement.Price.Currency;
        response.Price.Extra = advertisement.Price.Extra;
        response.Price.Value = advertisement.Price.Value;
        return response;
    }
}
