using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Shared;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromYesterdayConverter : IAvitoDateConverter
{
    private const string pattern = "вчер";
    private readonly string _date;

    public FromYesterdayConverter(string date)
    {
        _date = date;
    }

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromYesterdayConverter));

        string[] parts = _date.Split(' ', StringSplitOptions.TrimEntries);

        return parts.Any(p => AvitoConvertersShared.IsStringStartsWith(p, pattern))
            ? DateTime.Now.AddDays(-1)
            : AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromYesterdayConverter));
    }
}
