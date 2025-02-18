using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class MongoOwnerFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        OwnerInformationDto? info = dto.OwnerInformation;
        if (info == null)
            return;

        var builder = Builders<TransportAdvertisement>.Filter;

        if (!string.IsNullOrWhiteSpace(info.Status))
            filters.Add(builder.Eq(ad => ad.OwnerInformation.Status, info.Status));

        if (!string.IsNullOrWhiteSpace(info.Text))
            filters.Add(builder.Text(info.Text));
    }
}
