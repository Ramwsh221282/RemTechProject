using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementPriceInformation
{
    private const string NDS = "с НДС";
    private const string NO_NDS = "без НДС";
    public const int MAX_PRICE_EXTRA_LENGTH = 7;

    public string Extra { get; }
    public long Value { get; }

    private AdvertisementPriceInformation()
    {
        Extra = string.Empty;
        Value = 0;
    } // ef core

    private AdvertisementPriceInformation(string priceExtra, long priceValue) =>
        (Extra, Value) = (priceExtra, priceValue);

    public static Result<AdvertisementPriceInformation> Create(string? priceExtra, long priceValue)
    {
        if (priceValue <= 0)
            return new Error($"Цена объявления некорректна - {priceValue}.");

        return IsNDS(priceExtra)
            ? new AdvertisementPriceInformation(NDS, priceValue)
            : new AdvertisementPriceInformation(NO_NDS, priceValue);
    }

    private static bool IsNDS(string? price) =>
        price != null && price.Contains(NDS, StringComparison.OrdinalIgnoreCase);
}
