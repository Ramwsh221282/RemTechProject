using System.Text.RegularExpressions;
using MongoDB.Bson;
using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class AdvertisementAddressFilterQuery
    : IMongoFilterQuery<FilterAdvertisementsDto, TransportAdvertisement>
{
    public void AddFilter(
        FilterAdvertisementsDto dto,
        List<FilterDefinition<TransportAdvertisement>> filters
    )
    {
        AddressDto? address = dto.Address;
        if (address == null)
            return;

        if (string.IsNullOrWhiteSpace(address.Text))
            return;

        string input = address.Text;
        string regExp = $".*{Regex.Escape(input)}.*";

        var filter = new BsonDocument(
            "Address",
            new BsonDocument() { { "$regex", regExp }, { "$options", "i" } }
        );

        filters.Add(filter);
    }
}
