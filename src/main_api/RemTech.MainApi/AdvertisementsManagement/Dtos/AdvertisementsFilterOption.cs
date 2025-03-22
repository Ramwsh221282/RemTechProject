namespace RemTech.MainApi.AdvertisementsManagement.Dtos;

public sealed record AdvertisementsFilterOption(
    string? Title = null,
    string? Description = null,
    long? Price = null,
    string? PriceExtra = null,
    string? ServiceName = null,
    string? SourceUrl = null,
    string? Publisher = null,
    string? Address = null,
    DateTime? CreatedAt = null,
    DateTime? AdvertisementDate = null,
    AdvertisementCharacteristicFilterOption[]? Characteristics = null
);
