using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedPublisher
{
    public string Name { get; }
    public string Description { get; }

    public static ScrapedPublisher Default => new(string.Empty, string.Empty);

    private ScrapedPublisher(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public static Result<ScrapedPublisher> Create(string? name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Publisher name is empty");
        if (string.IsNullOrWhiteSpace(description))
            return new Error("Publisher description is empty");
        return new ScrapedPublisher(name, description);
    }
}
