using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class DescriptionSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly DescriptionDto? _dto;

    public DescriptionSpecification(DescriptionDto? dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_dto == null)
            return true;

        return entity.Description.Text.Contains(
            _dto.Description,
            StringComparison.OrdinalIgnoreCase
        );
    }
}
