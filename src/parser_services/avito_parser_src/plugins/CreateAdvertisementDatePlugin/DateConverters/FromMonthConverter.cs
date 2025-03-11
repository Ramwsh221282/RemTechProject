using System.Text.RegularExpressions;
using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromMonthConverter : IDateConverter
{
    private static readonly Dictionary<int, string> dates = new()
    {
        { 1, "янв" },
        { 2, "фев" },
        { 3, "мар" },
        { 4, "апр" },
        { 5, "ма" },
        { 6, "июн" },
        { 7, "июл" },
        { 8, "авг" },
        { 9, "сен" },
        { 10, "окт" },
        { 11, "ноя" },
        { 12, "дек" },
    };

    private readonly string _date;

    public FromMonthConverter(string date)
    {
        _date = date;
    }

    public Result<DateTime> ConvertToDateTime()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return new Error("String is empty");

        Result<int> monthMatch = HasAnyMonthSample();
        if (monthMatch.IsFailure)
            return monthMatch.Error;

        Result<int> dayMatch = GetDayBeforeMonth(monthMatch.Value);
        if (dayMatch.IsFailure)
            return dayMatch.Error;

        int year = DateTime.Now.Year;
        return new DateTime(year, monthMatch.Value, dayMatch.Value);
    }

    private Result<int> HasAnyMonthSample()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            foreach (var date in dates)
            {
                if (word.StartsWith(date.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return date.Key;
                }
            }
        }

        return new Error("Can't convert from day converter");
    }

    private Result<int> GetDayBeforeMonth(int matchedMonth)
    {
        string monthText = dates[matchedMonth];
        string pattern = @$"(\d{{1,2}})\s+{monthText}";

        Match match = Regex.Match(_date, pattern, RegexOptions.IgnoreCase);

        if (match.Success && int.TryParse(match.Groups[1].Value, out int day))
        {
            if (day >= 1 && day <= DateTime.DaysInMonth(DateTime.Now.Year, matchedMonth))
                return day;

            return new Error("Day is out of valid range");
        }

        return new Error("Day not found or invalid format");
    }
}
