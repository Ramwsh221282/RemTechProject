using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class OwnerInformationSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly OwnerInformationDto? _ownerInfo;

    public OwnerInformationSpecification(OwnerInformationDto? ownerInfo) => _ownerInfo = ownerInfo;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_ownerInfo == null)
            return true;

        bool matchDescription =
            string.IsNullOrWhiteSpace(_ownerInfo.Text)
            || entity.OwnerInformation.Text.Contains(
                _ownerInfo.Text,
                StringComparison.OrdinalIgnoreCase
            );

        bool matchStatus =
            string.IsNullOrWhiteSpace(_ownerInfo.Status)
            || entity.OwnerInformation.Text.Contains(
                _ownerInfo.Status,
                StringComparison.OrdinalIgnoreCase
            );

        return matchDescription && matchStatus;
    }
}
