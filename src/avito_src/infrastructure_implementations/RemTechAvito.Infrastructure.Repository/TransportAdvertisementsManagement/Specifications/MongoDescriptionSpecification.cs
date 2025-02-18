using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public sealed class MongoDescriptionSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly DescriptionDto? _dto;

    public MongoDescriptionSpecification(DescriptionDto? dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        if (_dto == null)
            return Builders<TransportAdvertisement>.Filter.Empty;

        var builder = Builders<TransportAdvertisement>.Filter;
        return builder.Text(_dto.Description);
    }
}
