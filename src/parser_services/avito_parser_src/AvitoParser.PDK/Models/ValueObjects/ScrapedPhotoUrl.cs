using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedPhotoUrl
{
    public string SourceUrl { get; }

    private ScrapedPhotoUrl(string sourceUrl) => SourceUrl = sourceUrl;

    public static Result<ScrapedPhotoUrl> Create(string? url) =>
        string.IsNullOrWhiteSpace(url) ? new Error("Photo url is empty") : new ScrapedPhotoUrl(url);
}
