using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class PriceSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly PriceFilterDto? _price;

    public PriceSpecification(PriceFilterDto? price)
    {
        _price = price;
    }

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_price == null)
            return true;

        bool matches = _price.Predicate switch
        {
            "LESS" => (entity.Price.Value < _price.Price.Value),
            "MORE" => (entity.Price.Value > _price.Price.Value),
            "EQUAL" => (entity.Price.Value == _price.Price.Value),
            _ => true,
        };

        return matches;
    }
}
