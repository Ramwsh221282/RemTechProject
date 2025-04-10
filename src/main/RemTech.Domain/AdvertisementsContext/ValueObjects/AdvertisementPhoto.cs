using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementPhoto
{
    public string Source { get; }

    private AdvertisementPhoto(string source) => Source = source;

    public static Result<AdvertisementPhoto> Create(string? source)
    {
        if (string.IsNullOrWhiteSpace(source))
            return ErrorFactory.EmptyOrNull(nameof(AdvertisementPhoto));

        return new AdvertisementPhoto(source);
    }
}
