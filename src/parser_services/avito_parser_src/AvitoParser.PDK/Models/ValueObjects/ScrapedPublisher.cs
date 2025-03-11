using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedPublisher
{
    public string Name { get; }

    public static ScrapedPublisher Default => new(string.Empty);

    private ScrapedPublisher(string name)
    {
        Name = name;
    }

    public static Result<ScrapedPublisher> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Publisher name is empty");
        return new ScrapedPublisher(name);
    }
}
