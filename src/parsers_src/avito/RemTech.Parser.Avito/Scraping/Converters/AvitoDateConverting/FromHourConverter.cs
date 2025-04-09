using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromHourConverter : IAvitoDateConverter
{
    private const string sample = "час";
    private readonly string _date;

    public FromHourConverter(string date) => _date = date;

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.StringWasNullOrEmpty(nameof(FromHourConverter));

        return HasHourSample()
            ? DateTime.Now
            : AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromHourConverter));
    }

    private bool HasHourSample()
    {
        string[] words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Any(word => word.StartsWith(sample, StringComparison.OrdinalIgnoreCase));
    }
}
