using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record AdvertisementDate
{
    public ulong UnixTime { get; }
    private AdvertisementDate(ulong unixTime) => UnixTime = unixTime;

    public static Result<AdvertisementDate> Create(ulong unixTime) =>
        unixTime <= 0 ? new Error("Invalid date value") : new AdvertisementDate(unixTime);
}