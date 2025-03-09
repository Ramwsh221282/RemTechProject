using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromWeekConverter : IDateConverter
{
    private const string WeekSample = "недел";
    private readonly string _date;

    public FromWeekConverter(string date) => _date = date;

    public Result<DateTime> ConvertToDateTime()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return new Error("String is empty");
        if (!HasWeekSample())
            return new Error("Cannot convert from week converter");
        return GetDateWithWeekDifference();
    }

    private bool HasWeekSample()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (word.StartsWith(WeekSample, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private DateTime GetDateWithWeekDifference()
    {
        int difference = _date.GetIntegerFromString() * 7;
        DateTime current = DateTime.Now;
        return DateTime.Now.AddDays(-difference);
    }
}
