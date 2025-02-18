namespace RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;

public sealed record FilterAdvertisementsDto(
    CharacteristicsListDto? Characteristics = null,
    AddressDto? Address = null,
    OwnerInformationDto? OwnerInformation = null,
    PriceFilterDto? Price = null,
    DescriptionDto? Description = null,
    DateFilterDto? Date = null
);

public sealed record PriceFilterDto(PriceDto Price, string Predicate);

public sealed record DateFilterDto(DateOnly Date, string Predicate);
