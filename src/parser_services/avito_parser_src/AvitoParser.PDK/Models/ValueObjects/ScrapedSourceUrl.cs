using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedSourceUrl
{
    public string SourceUrl { get; }

    public static ScrapedSourceUrl Default => new(string.Empty);

    private ScrapedSourceUrl(string sourceUrl)
    {
        SourceUrl = sourceUrl;
    }

    public static Result<ScrapedSourceUrl> Create(string? sourceUrl)
    {
        if (string.IsNullOrWhiteSpace(sourceUrl))
            return new Error("Source url is empty");
        return new ScrapedSourceUrl(sourceUrl);
    }
}
