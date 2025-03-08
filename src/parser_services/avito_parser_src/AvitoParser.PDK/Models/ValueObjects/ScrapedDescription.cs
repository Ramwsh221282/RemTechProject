using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedDescription
{
    public string Description { get; }
    public static ScrapedDescription Default => new(string.Empty);

    private ScrapedDescription(string description)
    {
        Description = description;
    }

    public static Result<ScrapedDescription> Create(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return new Error("Description is empty");
        return new ScrapedDescription(description);
    }
}
