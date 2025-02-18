using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class MongoCharacteristicsSearchQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        CharacteristicsListDto? characteristics = dto.CharacteristicsSearch;
        if (characteristics == null)
            return;

        List<string> textSearch = new List<string>();
        var builder = Builders<TransportAdvertisement>.Filter;
        foreach (var ctx in characteristics.Characteristics)
        {
            if (!string.IsNullOrWhiteSpace(ctx.Name))
                textSearch.Add(ctx.Name);

            if (!string.IsNullOrWhiteSpace(ctx.Value))
                textSearch.Add(ctx.Value);
        }

        if (textSearch.Count > 0)
            filters.Add(builder.Text(string.Join(" ", textSearch)));
    }
}
