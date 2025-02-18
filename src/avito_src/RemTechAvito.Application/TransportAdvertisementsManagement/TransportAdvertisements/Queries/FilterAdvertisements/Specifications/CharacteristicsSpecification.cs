using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class CharacteristicsSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly CharacteristicsListDto? _characteristics;

    public CharacteristicsSpecification(CharacteristicsListDto? characteristics) =>
        _characteristics = characteristics;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_characteristics == null)
            return true;

        var nameMatch = entity.Characteristics.Data.Any(c =>
            _characteristics.Characteristics.Any(d =>
                d.Name.Contains(c.Name, StringComparison.OrdinalIgnoreCase)
            )
        );

        var valueMatch = entity.Characteristics.Data.Any(c =>
            _characteristics.Characteristics.Any(d =>
                d.Value.Contains(c.Name, StringComparison.OrdinalIgnoreCase)
            )
        );

        return nameMatch && valueMatch;
    }
}
