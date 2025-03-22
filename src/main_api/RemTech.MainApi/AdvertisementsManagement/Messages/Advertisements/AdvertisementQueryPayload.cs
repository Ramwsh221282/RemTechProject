using RemTech.MainApi.AdvertisementsManagement.Dtos;

namespace RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;

public sealed record AdvertisementQueryPayload(
    long? AdvertisementId = null,
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
    AdvertisementCharacteristicsQueryPayload[]? Characteristics = null
)
{
    public bool IsPayloadEmpty =>
        AdvertisementId == null
        && Title == null
        && Description == null
        && Price == null
        && PriceExtra == null
        && ServiceName == null
        && SourceUrl == null
        && Publisher == null
        && Address == null
        && CreatedAt == null
        && AdvertisementDate == null
        && Characteristics == null;
}

public static class AdvertisementQueryPayloadExtensions
{
    public static AdvertisementQueryPayload FromFilterOption(
        this AdvertisementsFilterOption option
    ) =>
        new(
            Title: option.Title,
            Description: option.Description,
            Price: option.Price,
            PriceExtra: option.PriceExtra,
            ServiceName: option.ServiceName,
            SourceUrl: option.SourceUrl,
            Publisher: option.Publisher,
            Address: option.Address,
            CreatedAt: option.CreatedAt,
            AdvertisementDate: option.AdvertisementDate,
            Characteristics: option.Characteristics == null
                ? null
                : [.. option.Characteristics.Select(FromFilterOption)]
        );

    public static AdvertisementCharacteristicsQueryPayload FromFilterOption(
        this AdvertisementCharacteristicFilterOption option
    ) => new(option.Name, option.Value);
}
