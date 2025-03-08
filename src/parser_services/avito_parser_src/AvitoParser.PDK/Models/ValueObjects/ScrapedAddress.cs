using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public sealed record ScrapedAddress
{
    public string Address { get; }

    private ScrapedAddress(string address) => Address = address;

    public static ScrapedAddress Default => new ScrapedAddress(string.Empty);

    public static Result<ScrapedAddress> Create(string? parsedAddress) =>
        string.IsNullOrWhiteSpace(parsedAddress)
            ? new Error("Parsed address string is empty")
            : new ScrapedAddress(parsedAddress);
}
