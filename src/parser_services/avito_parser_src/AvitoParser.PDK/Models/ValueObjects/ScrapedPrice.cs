using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedPrice
{
    public ulong Price { get; }
    public string Description { get; }
    public static ScrapedPrice Default => new(0, string.Empty);

    private ScrapedPrice(ulong price, string description)
    {
        Price = price;
        Description = description;
    }

    public static Result<ScrapedPrice> Create(string? price, string? description)
    {
        if (string.IsNullOrEmpty(price))
            return new Error("Price string is empty");
        if (string.IsNullOrEmpty(description))
            return new Error("Description is empty");
        bool canConvert = ulong.TryParse(price, out ulong parsedPrice);
        if (!canConvert)
            return new Error("Price string is not a number");
        return new ScrapedPrice(parsedPrice, description);
    }
}
