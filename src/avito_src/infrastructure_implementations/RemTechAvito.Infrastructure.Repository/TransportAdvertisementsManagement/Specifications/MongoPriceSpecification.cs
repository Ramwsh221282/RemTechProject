using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public sealed class MongoPriceSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly PriceFilterDto? _dto;

    public MongoPriceSpecification(PriceFilterDto? dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        if (_dto == null)
            return Builders<TransportAdvertisement>.Filter.Empty;

        var builder = Builders<TransportAdvertisement>.Filter;
        return _dto.Predicate switch
        {
            "LESS" => builder.Lt(ad => ad.Price.Value, _dto.Price.Value),
            "MORE" => builder.Gt(ad => ad.Price.Value, _dto.Price.Value),
            "EQUAL" => builder.Eq(ad => ad.Price.Value, _dto.Price.Value),
            _ => builder.Empty,
        };
    }
}
