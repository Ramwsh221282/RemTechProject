using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class TransportAdvertisementSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly ISpecification<TransportAdvertisement> _combined;

    public TransportAdvertisementSpecification(FilterAdvertisementsDto dto)
    {
        var specs = new List<ISpecification<TransportAdvertisement>>();

        if (dto.Characteristics != null)
            specs.Add(new CharacteristicsSpecification(dto.Characteristics));

        if (dto.Address != null)
            specs.Add(new AddressSpecification(dto.Address));

        if (dto.OwnerInformation != null)
            specs.Add(new OwnerInformationSpecification(dto.OwnerInformation));

        if (dto.Price != null)
            specs.Add(new PriceSpecification(dto.Price));

        if (dto.Description != null)
            specs.Add(new DescriptionSpecification(dto.Description));

        if (dto.Date != null)
            specs.Add(new DateSpecification(dto.Date));

        var combinedSpec = specs.FirstOrDefault()!;
        foreach (var spec in specs.Skip(1))
        {
            combinedSpec = combinedSpec.And(spec);
        }

        _combined = combinedSpec;
    }

    public override bool IsSatisfiedBy(TransportAdvertisement entity) =>
        _combined.IsSatisfiedBy(entity);

    public ISpecification<TransportAdvertisement> GetSpecification() => _combined;
}
