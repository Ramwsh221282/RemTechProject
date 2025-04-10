using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromDayConverter(string date) : IAvitoDateConverter
{
    private static readonly string[] samples = ["ден", "дня", "дне"];
    private readonly string _date = date;

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.StringWasNullOrEmpty(nameof(FromDayConverter));

        if (!HasAnySample())
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromDayConverter));

        return GetDateWithOneDayDifference();
    }

    private bool HasAnySample()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (samples.Any(sample => word.StartsWith(sample, StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }

    private DateTime GetDateWithOneDayDifference()
    {
        int daysDifference = _date.GetIntegerFromString();
        return DateTime.Now.AddDays(-daysDifference);
    }
}
