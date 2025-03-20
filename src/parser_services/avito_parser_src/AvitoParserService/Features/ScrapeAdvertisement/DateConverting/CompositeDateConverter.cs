using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public sealed class CompositeDateConverter : IDateConverter
{
    private readonly string _date;
    private readonly List<IDateConverter> _converters;

    public CompositeDateConverter(string date)
    {
        _date = date;
        _converters =
        [
            new FromDayConverter(_date),
            new FromYesterdayConverter(_date),
            new FromHourConverter(_date),
            new FromMonthConverter(_date),
            new FromMonthWithYearConverter(_date),
            new FromTodayConverter(_date),
            new FromWeekConverter(_date),
        ];
    }

    public Option<DateTime> Convert()
    {
        foreach (var converter in _converters)
        {
            Option<DateTime> conversion = converter.Convert();
            if (conversion.HasValue)
                return conversion;
        }

        return Option<DateTime>.None();
    }
}
