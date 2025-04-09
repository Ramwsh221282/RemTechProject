using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromWeekConverter : IAvitoDateConverter
{
    private const string pattern = "недел";
    private readonly string _date;

    public FromWeekConverter(string date)
    {
        _date = date;
    }

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.StringWasNullOrEmpty(nameof(FromWeekConverter));

        if (!HasWeekSample())
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromWeekConverter));

        return GetDateWithWeekDifference();
    }

    private bool HasWeekSample()
    {
        string[] words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Any(word => word.StartsWith(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private DateTime GetDateWithWeekDifference()
    {
        int difference = _date.GetIntegerFromString() * 7;
        return DateTime.Now.AddDays(-difference);
    }
}
