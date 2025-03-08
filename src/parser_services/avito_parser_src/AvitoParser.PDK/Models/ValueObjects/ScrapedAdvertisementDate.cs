using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public record ScrapedAdvertisementDate
{
    public ulong UnixTime { get; }

    public static ScrapedAdvertisementDate Default => new(0);

    public ScrapedAdvertisementDate(ulong unixTime) => UnixTime = unixTime;

    public static Result<ScrapedAdvertisementDate> Create(ulong unixTime) =>
        unixTime == 0
            ? new Error("Invalid unix time representation")
            : new ScrapedAdvertisementDate(unixTime);
}
