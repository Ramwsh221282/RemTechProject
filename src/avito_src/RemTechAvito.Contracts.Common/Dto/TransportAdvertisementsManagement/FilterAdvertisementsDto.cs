namespace RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;

public sealed record FilterAdvertisementsDto(
    CharacteristicsListDto? Characteristics = null,
    AddressDto? Address = null,
    PriceFilterDto? Price = null,
    PriceRangeDto? PriceRange = null,
    TextSearchDto? Text = null
);

public sealed record PriceRangeDto(uint ValueMin, uint ValueMax);

public sealed record PriceFilterDto(PriceDto Price, string Predicate);

public sealed record DateFilterDto(DateOnly Date, string Predicate);

public sealed record TextSearchDto(string Text);
