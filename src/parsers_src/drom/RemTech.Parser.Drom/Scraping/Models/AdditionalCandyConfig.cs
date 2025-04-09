using System.Text.Json;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.Models;

public sealed record AdditionalCandyConfig(
    long Price,
    string Brand,
    string Model,
    string City,
    string Region,
    int Year
) : IDromScrapedAdvertisementProperty
{
    public ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic releaseYear = new("Год выпуска", Year.ToString());
        string combinedAddress = string.Join(", ", Region, City);
        string title = $"{Brand} {Model} {Year}";
        return advertisement with
        {
            Title = title,
            Price = Price,
            Characteristics = [releaseYear, .. advertisement.Characteristics],
            Address = combinedAddress,
        };
    }
}

public static class AdditionalCandyConfigFactory
{
    public static Result<AdditionalCandyConfig> FromJsonDocument(JsonDocument document)
    {
        try
        {
            bool hasCandyConfig = document.RootElement.TryGetProperty(
                "additionalCandyConfig",
                out JsonElement candyConfig
            );
            if (!hasCandyConfig)
                return new Error("Missing candy config property.");
            bool hasLocations = candyConfig.TryGetProperty("locations", out JsonElement locations);
            if (!hasLocations)
                return new Error("Missing locations property.");
            foreach (JsonElement item in locations.EnumerateArray())
            {
                bool hasParams = item.TryGetProperty("candyParams", out JsonElement candyParams);
                if (!hasParams)
                    return new Error("Missing params property.");
                long price = candyParams.GetProperty("sum").GetInt64();
                string? brand = candyParams.GetProperty("maker").GetString();
                if (string.IsNullOrWhiteSpace(brand))
                    return new Error("Missing brand property.");
                string? model = candyParams.GetProperty("model").GetString();
                if (string.IsNullOrWhiteSpace(model))
                    return new Error("Missing model property.");
                string? city = candyParams.GetProperty("city").GetString();
                if (string.IsNullOrWhiteSpace(city))
                    return new Error("Missing city property.");
                string? region = candyParams.GetProperty("region").GetString();
                if (string.IsNullOrWhiteSpace(region))
                    return new Error("Missing region property.");
                int year = candyParams.GetProperty("year").GetInt32();
                return new AdditionalCandyConfig(price, brand, model, city, region, year);
            }
            return new Error("Unable to parse Additional candy config.");
        }
        catch
        {
            Console.WriteLine($"Exception at {nameof(AdditionalCandyConfigFactory)}");
            return new Error("Exception");
        }
    }
}
