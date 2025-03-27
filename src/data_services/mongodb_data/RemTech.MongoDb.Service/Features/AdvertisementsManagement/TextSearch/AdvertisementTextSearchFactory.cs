using MongoDB.Bson;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.BsonRegularExpressionUtils;
using RemTech.MongoDb.Service.Common.Abstractions.FilterDefinitionUtils;
using RemTech.MongoDb.Service.Common.Abstractions.TextSearchFilterFactory;
using RemTech.MongoDb.Service.Common.Dtos;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.TextSearch;

public class AdvertisementTextSearchFactory : ITextSearchFilterFactory<Advertisement>
{
    public FilterDefinition<Advertisement> Apply(
        FilterDefinition<Advertisement> filter,
        TextSearchOption option
    )
    {
        string? term = option.SearchTerm;
        if (string.IsNullOrWhiteSpace(term))
            return filter;

        List<FilterDefinition<Advertisement>> filters =
        [
            ApplyForAddress(filter, term),
            ApplyForDescription(filter, term),
            ApplyForCharacteristics(filter, term),
            ApplyForTitle(filter, term),
        ];

        return Builders<Advertisement>.Filter.And(Builders<Advertisement>.Filter.Or(filters));
    }

    protected FilterDefinition<Advertisement> ApplyForTitle(
        FilterDefinition<Advertisement> filter,
        string term
    ) => filter.ApplyAnd(term.CreateBsonDocumentWithRegularExpressionFilter("Title"));

    protected FilterDefinition<Advertisement> ApplyForDescription(
        FilterDefinition<Advertisement> filter,
        string term
    ) => filter.ApplyAnd(term.CreateBsonDocumentWithRegularExpressionFilter("Description"));

    protected FilterDefinition<Advertisement> ApplyForAddress(
        FilterDefinition<Advertisement> filter,
        string term
    ) => filter.ApplyAnd(term.CreateBsonDocumentWithRegularExpressionFilter("Address"));

    protected FilterDefinition<Advertisement> ApplyForCharacteristics(
        FilterDefinition<Advertisement> filter,
        string term
    )
    {
        BsonDocument matchFilter = new BsonDocument(
            "$elemMatch",
            new BsonDocument()
            {
                {
                    "value",
                    new BsonDocument($"regex", term.CreateBsonRegularExpressionFromString())
                },
            }
        );
        BsonDocument finalFilter = new BsonDocument("Characteristics", matchFilter);
        return filter.ApplyAnd(finalFilter);
    }
}
