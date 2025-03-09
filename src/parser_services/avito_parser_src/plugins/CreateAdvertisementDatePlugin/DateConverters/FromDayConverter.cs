using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromDayConverter : IDateConverter
{
    private static readonly string[] samples = ["ден", "дня", "дне"];
    private readonly string _date;

    public FromDayConverter(string date) => _date = date;

    public Result<DateTime> ConvertToDateTime()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return new Error("Date string is empty");

        if (!HasAnySample())
            return new Error("Can't convert from day converter");

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
