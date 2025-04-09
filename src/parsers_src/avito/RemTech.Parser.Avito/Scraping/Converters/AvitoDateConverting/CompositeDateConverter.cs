using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class CompositeDateConverter : IAvitoDateConverter
{
    private readonly List<IAvitoDateConverter> _converters;

    public CompositeDateConverter(string date)
    {
        _converters =
        [
            new FromDayConverter(date),
            new FromYesterdayConverter(date),
            new FromHourConverter(date),
            new FromMonthConverter(date),
            new FromMonthWithYearConverter(date),
            new FromTodayConverter(date),
            new FromWeekConverter(date),
        ];
    }

    public Result<DateTime> Convert()
    {
        foreach (IAvitoDateConverter converter in _converters)
        {
            Result<DateTime> conversion = converter.Convert();
            if (conversion.IsSuccess)
                return conversion;
        }

        return AvitoDateConvertingErrors.NotMatchingPattern(nameof(CompositeDateConverter));
    }
}
