using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class AdvertisementCharacteristicsFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        var criterias = dto.Characteristics;
        if (criterias == null)
            return;

        foreach (var criteria in criterias.Characteristics)
        {
            if (string.IsNullOrWhiteSpace(criteria.Name))
                continue;
            if (string.IsNullOrWhiteSpace(criteria.Value))
                continue;

            string regExp = $".*{Regex.Escape(criteria.Value)}.*";

            var filter = new BsonDocument(
                "Characteristics",
                new BsonDocument(
                    "$elemMatch",
                    new BsonDocument
                    {
                        { "attribute_name", criteria.Name },
                        {
                            "attribute_value",
                            new BsonDocument { { "$regex", regExp }, { "$options", "i" } }
                        },
                    }
                )
            );

            filters.Add(filter);
        }
    }
}
