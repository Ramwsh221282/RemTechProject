using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedAdvertisementId
{
    public ulong Id { get; }
    public static ScrapedAdvertisementId Default => new(0);

    private ScrapedAdvertisementId(ulong id) => Id = id;

    public static Result<ScrapedAdvertisementId> Create(string? parsedId)
    {
        if (string.IsNullOrWhiteSpace(parsedId))
            return new Error("Parsed advertisement id is empty");
        bool canConvert = ulong.TryParse(parsedId, out ulong id);
        if (!canConvert)
            return new Error("Cannot convert to ulong id representation");
        return new ScrapedAdvertisementId(id);
    }
}
