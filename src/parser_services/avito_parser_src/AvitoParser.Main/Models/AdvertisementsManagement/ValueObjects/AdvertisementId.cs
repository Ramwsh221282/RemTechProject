using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record AdvertisementId
{
    public ulong Value { get; }

    private AdvertisementId(ulong value) => Value = value;

    public static Result<AdvertisementId> Create(ulong value) =>
        value <= 0 ? new Error("Advertisement id is less or equal 0") : new AdvertisementId(value);

    public static Result<AdvertisementId> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Advertisement id is empty");

        bool canConvert = ulong.TryParse(value, out ulong id);
        if (!canConvert)
            return new Error("Cannot convert advertisement to ulong id value");

        return Create(id);
    }
}
