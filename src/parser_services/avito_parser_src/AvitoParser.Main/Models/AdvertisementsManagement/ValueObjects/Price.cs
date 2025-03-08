using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record Price
{
    public ulong Value { get; }
    public string AdditionInfo { get; } = string.Empty;

    private Price(ulong value, string? additionInfo = null)
    {
        Value = value;
        AdditionInfo = additionInfo ?? string.Empty;
    }

    public static Result<Price> Create(ulong value, string? additionInfo = null) =>
        value <= 0 ? new Error("Price value is less or equal 0") : new Price(value, additionInfo);
}
