using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class AddressSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly AddressDto? _address;

    public AddressSpecification(AddressDto? address) => _address = address;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_address == null)
            return true;

        return _address.Text.Contains(entity.Address.Text, StringComparison.OrdinalIgnoreCase);
    }
}
