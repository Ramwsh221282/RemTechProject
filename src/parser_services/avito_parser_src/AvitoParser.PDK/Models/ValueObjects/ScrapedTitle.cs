using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public sealed record ScrapedTitle
{
    public string Title { get; }

    public static ScrapedTitle Default => new(string.Empty);

    private ScrapedTitle(string title) => Title = title;

    public static Result<ScrapedTitle> Create(string? scrapedTitle) =>
        string.IsNullOrWhiteSpace(scrapedTitle)
            ? new Error("Title is empty")
            : new ScrapedTitle(scrapedTitle);
}
