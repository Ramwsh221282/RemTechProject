using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public record SourceUrl
{
    public string Url { get; }
    private SourceUrl(string url) => Url = url;

    public static Result<SourceUrl> Create(string? url) =>
        UrlLinkValidator.IsStringUrl(url) ? new SourceUrl(url!) : new Error("Invalid url");
}