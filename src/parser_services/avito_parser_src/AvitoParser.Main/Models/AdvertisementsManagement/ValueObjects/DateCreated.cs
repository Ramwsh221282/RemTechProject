using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record DateCreated
{
    public ulong UnixTime { get; }
    private DateCreated(ulong unixTime) => UnixTime = unixTime;

    public static Result<DateCreated> Create(ulong unixTime) =>
        unixTime <= 0 ? new Error("Unix date is invalid") : new DateCreated(unixTime);
}