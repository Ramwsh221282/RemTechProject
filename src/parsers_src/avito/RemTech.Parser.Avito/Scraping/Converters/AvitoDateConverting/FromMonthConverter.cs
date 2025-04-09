using System.Text.RegularExpressions;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed class FromMonthConverter(string date) : IAvitoDateConverter
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

    private readonly string _date = date;

    public Result<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return AvitoDateConvertingErrors.StringWasNullOrEmpty(nameof(FromMonthConverter));

        Result<int> month = GetMonthNumber();
        if (month.IsFailure)
            return month.Error;

        Result<int> day = GetDayBeforeMonth(month.Value);
        if (day.IsFailure)
            return day.Error;

        int year = DateTime.Now.Year;
        return new DateTime(year, month.Value, day.Value);
    }

    private Result<int> GetMonthNumber()
    {
        string[] words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            foreach (
                KeyValuePair<int, string> date in dates.Where(date =>
                    word.StartsWith(date.Value, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                return date.Key;
            }
        }

        return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromMonthConverter));
    }

    private Result<int> GetDayBeforeMonth(int matchedMonth)
    {
        string monthText = dates[matchedMonth];
        string pattern = @$"(\d{{1,2}})\s+{monthText}";

        Match match = Regex.Match(_date, pattern, RegexOptions.IgnoreCase);

        if (!match.Success || !int.TryParse(match.Groups[1].Value, out int day))
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromMonthConverter));
        if (day >= 1 && day <= DateTime.DaysInMonth(DateTime.Now.Year, matchedMonth))
            return day;

        return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromMonthConverter));
    }
}
