using System.Text.RegularExpressions;
using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromMonthWithYearConverter : IDateConverter
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

    private const string pattern = @"(\d{1,2})\s+([\p{L}]+)\s+(\d{4})";
    private static readonly Regex RegexPattern = new Regex(pattern, RegexOptions.Compiled);
    private readonly string _date;

    public FromMonthWithYearConverter(string date)
    {
        _date = date;
    }

    public Result<DateTime> ConvertToDateTime()
    {
        return string.IsNullOrWhiteSpace(_date) ? new Error("Date string is empty") : ConvertDate();
    }

    private Result<DateTime> ConvertDate()
    {
        Match match = RegexPattern.Match(_date);
        if (!match.Success)
            return new Error("Can't convert date from month with year converter");
        string dayString = match.Groups[1].Value;
        string monthString = match.Groups[2].Value;
        string yearString = match.Groups[3].Value;
        int yearNumber = int.Parse(yearString);
        var monthPair = dates.FirstOrDefault(d =>
            monthString.StartsWith(d.Value, StringComparison.OrdinalIgnoreCase)
        );
        int monthNumber = monthPair.Key;
        int dayNumber = int.Parse(dayString);
        return new DateTime(yearNumber, monthNumber, dayNumber);
    }
}
