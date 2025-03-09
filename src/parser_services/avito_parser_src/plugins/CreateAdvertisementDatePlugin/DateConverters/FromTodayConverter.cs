using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromTodayConverter : IDateConverter
{
    private const string sample = "сегодн";
    private readonly string _date;

    public FromTodayConverter(string date)
    {
        _date = date;
    }

    public Result<DateTime> ConvertToDateTime()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return new Error("String is empty");
        string[] parts = _date.Split(' ');
        if (parts.Any(p => p.StartsWith(sample, StringComparison.OrdinalIgnoreCase)))
            return DateTime.Now;
        return new Error("Cannot convert from today converted");
    }
}
