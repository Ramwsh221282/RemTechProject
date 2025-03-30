using System.Text.Json;
using RemTechCommon.Utils.ResultPattern;
using SharedParsersLibrary.Models;

namespace DromScrapingTests.DromDeserializationModels;

public sealed record GeoInfo(string Publisher) : IDromScrapedAdvertisementProperty
{
    public ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedAdvertisement clone = advertisement.Clone();
        clone.Publisher = Publisher;
        return clone;
    }
}

public static class GeoInfoFactory
{
    public static Result<GeoInfo> FromJsonDocument(JsonDocument document)
    {
        try
        {
            bool hasGeoInfo = document.RootElement.TryGetProperty("geoInfo", out JsonElement _);
            if (!hasGeoInfo)
                return new Error("Missing geo info property.");
            bool hasDealerCard = document.RootElement.TryGetProperty(
                "dealerCard",
                out JsonElement dealerCard
            );
            if (
                !hasDealerCard
                || dealerCard.ValueKind == JsonValueKind.Null
                || dealerCard.ValueKind == JsonValueKind.Undefined
            )
                return new GeoInfo("Частное лицо");
            bool hasDealerCardText = dealerCard.TryGetProperty(
                "dealerCardText",
                out JsonElement dealerCardText
            );
            if (!hasDealerCardText)
                return new Error("Missing dealer card text property.");
            string? dealerText = dealerCardText.GetString();
            if (string.IsNullOrWhiteSpace(dealerText))
                return new Error("Missing dealer card text property.");
            return new GeoInfo(dealerText);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception at {nameof(GeoInfoFactory)}");
            return new Error("Exception");
        }
    }
}
