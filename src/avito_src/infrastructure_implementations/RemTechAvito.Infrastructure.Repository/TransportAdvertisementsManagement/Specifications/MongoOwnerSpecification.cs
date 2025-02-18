using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public sealed class MongoOwnerSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly OwnerInformationDto? _dto;

    public MongoOwnerSpecification(OwnerInformationDto? dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        if (_dto == null)
            return Builders<TransportAdvertisement>.Filter.Empty;

        var builder = Builders<TransportAdvertisement>.Filter;
        var filters = new List<FilterDefinition<TransportAdvertisement>>();

        if (!string.IsNullOrWhiteSpace(_dto.Status))
            filters.Add(builder.Eq(ad => ad.OwnerInformation.Status, _dto.Status));

        if (!string.IsNullOrWhiteSpace(_dto.Text))
            filters.Add(builder.Text(_dto.Text));

        return builder.And(filters);
    }
}
