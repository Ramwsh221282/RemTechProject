using System.Globalization;
using System.Text.Json;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.Models;

public sealed record BullCounters(DateTime Date) : IDromScrapedAdvertisementProperty
{
    public ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        return advertisement with { Published = Date };
    }
}

public static class BullCountersFactory
{
    public static Result<BullCounters> FromJsonDocument(JsonDocument document)
    {
        try
        {
            bool hasBullCounters = document.RootElement.TryGetProperty(
                "bullCounters",
                out JsonElement bullCounters
            );

            if (!hasBullCounters)
                return new Error("Bull counters was not found.");

            bool hasDateProperty = bullCounters.TryGetProperty(
                "bullDate",
                out JsonElement bullDate
            );

            if (!hasDateProperty)
                return new Error("Bull date property was not found.");

            string? bullDateValue = bullDate.GetString();

            if (string.IsNullOrWhiteSpace(bullDateValue))
                return new Error("Bull date is empty.");

            DateTime date = DateTime.ParseExact(
                bullDateValue,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture
            );

            return new BullCounters(date);
        }
        catch
        {
            Console.WriteLine($"Error at {nameof(BullCountersFactory)}");
            return new Error("Exception");
        }
    }
}
