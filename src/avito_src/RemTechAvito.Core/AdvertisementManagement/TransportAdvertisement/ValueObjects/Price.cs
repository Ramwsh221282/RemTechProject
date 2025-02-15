using RemTechCommon.Utils.Converters;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record Price
{
    public uint Value { get; }
    public string Currency { get; }
    public string Extra { get; }

    private Price(uint value, string currency, string? extra)
    {
        Value = value;
        Currency = currency;
        Extra = string.IsNullOrWhiteSpace(extra) ? String.Empty : extra;
    }

    public static Result<Price> Create(uint value, string? currency, string? extra) =>
        string.IsNullOrWhiteSpace(currency)
            ? new Error("Currency should be provided")
            : new Price(value, currency, extra);

    public static Result<Price> Create(string? value, string currency, string extra)
    {
        Result<uint> priceValue = value.ToUint();
        return priceValue.IsFailure ? priceValue.Error : Create(priceValue, currency, extra);
    }
}
