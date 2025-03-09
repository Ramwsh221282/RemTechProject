using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class CompositeDateConverter : IDateConverter
{
    private readonly string _date;

    public CompositeDateConverter(string date) => _date = date;

    public Result<DateTime> ConvertToDateTime()
    {
        List<IDateConverter> converters = new List<IDateConverter>();
        converters.Add(new FromDayConverter(_date));
        converters.Add(new FromYesterdayConverter(_date));
        converters.Add(new FromHourConverter(_date));
        converters.Add(new FromMonthConverter(_date));
        converters.Add(new FromMonthWithYearConverter(_date));
        converters.Add(new FromTodayConverter(_date));
        converters.Add(new FromWeekConverter(_date));
        foreach (
            var result in converters
                .Select(converter => converter.ConvertToDateTime())
                .Where(result => result.IsSuccess)
        )
        {
            return result;
        }

        return new Error("Unable to parse avito date string");
    }
}
