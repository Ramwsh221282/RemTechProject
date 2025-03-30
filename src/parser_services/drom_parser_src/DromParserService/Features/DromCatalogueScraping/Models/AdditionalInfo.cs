using System.Text.Json;
using System.Text.RegularExpressions;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using SharedParsersLibrary.Models;

namespace DromParserService.Features.DromCatalogueScraping.Models;

public sealed partial record AdditionalInfo(string Description) : IDromScrapedAdvertisementProperty
{
    private static readonly Regex BrRemoveRegex = Regex();

    public ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedAdvertisement clone = advertisement.Clone();
        string formatted = Description
            .CleanString()
            .Replace("<br />", " ")
            .CleanString()
            .Replace("•", " ")
            .CleanString();
        formatted = BrRemoveRegex
            .Replace(formatted, " ")
            .CleanString()
            .ReplaceLineEndings()
            .CleanString();
        clone.Description = formatted;
        return clone;
    }

    [GeneratedRegex(
        @"<b />",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant
    )]
    private static partial Regex Regex();
}

public static class AdditionalInfoFactory
{
    public static Result<AdditionalInfo> FromJsonDocument(JsonDocument document)
    {
        try
        {
            bool hasAdditionalInfo = document.RootElement.TryGetProperty(
                "additionalInfo",
                out JsonElement additionalInfo
            );
            if (!hasAdditionalInfo)
                return new Error("Missing additional info property.");
            if (additionalInfo.ValueKind == JsonValueKind.Null)
                return new Error("Additional info property is null.");
            bool hasHidden = additionalInfo.TryGetProperty("hidden", out JsonElement hidden);
            if (!hasHidden)
                return additionalInfo.TryParseAsVisibleValue();
            if (hidden.ValueKind == JsonValueKind.Null)
                return additionalInfo.TryParseAsVisibleValue();
            string? text = hidden.GetString();
            if (string.IsNullOrWhiteSpace(text))
                return new Error("Description is empty.");
            return new AdditionalInfo(
                Regex.Unescape(text).CleanString().ReplaceLineEndings().CleanString()
            );
        }
        catch
        {
            Console.WriteLine($"Exception at {nameof(AdditionalInfoFactory)}");
            return new Error("Exception");
        }
    }

    private static Result<AdditionalInfo> TryParseAsVisibleValue(this JsonElement additionalInfo)
    {
        bool hasVisibleProperty = additionalInfo.TryGetProperty(
            "visible",
            out JsonElement visibleProperty
        );
        if (!hasVisibleProperty)
            return new Error("Missing visible property.");
        if (visibleProperty.ValueKind == JsonValueKind.Null)
            return new Error("Visible description property is null.");
        string? description = visibleProperty.GetString();
        if (string.IsNullOrWhiteSpace(description))
            return new Error("Visible description is empty.");
        return new AdditionalInfo(description.CleanString().ReplaceLineEndings().CleanString());
    }
}
