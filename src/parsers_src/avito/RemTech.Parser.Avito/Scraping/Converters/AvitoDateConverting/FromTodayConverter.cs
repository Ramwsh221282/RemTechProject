using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Shared;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromTodayConverter(string date) : IAvitoDateConverter
{
    private const string sample = "сегодн";
    private readonly string _date = date;

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.StringWasNullOrEmpty(nameof(FromTodayConverter));

        string[] parts = _date.Split(' ', StringSplitOptions.TrimEntries);

        return parts.Any(p => AvitoConvertersShared.IsStringStartsWith(p, sample))
            ? DateTime.Now
            : AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromTodayConverter));
    }
}
