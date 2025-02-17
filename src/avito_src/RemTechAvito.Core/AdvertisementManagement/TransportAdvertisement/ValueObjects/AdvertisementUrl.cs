using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record AdvertisementUrl
{
    public string Url { get; }

    private AdvertisementUrl(string url) => Url = url;

    public static Result<AdvertisementUrl> Create(string? url) =>
        string.IsNullOrWhiteSpace(url)
            ? new Error("Advertisement url is not provided")
            : new AdvertisementUrl(url);
}
