using System.Text.RegularExpressions;
using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

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

    public Option<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return Option<DateTime>.None();
        Option<int> month = GetMonthNumber();
        if (!month.HasValue)
            return Option<DateTime>.None();
        Option<int> day = GetDayBeforeMonth(month.Value);
        if (!day.HasValue)
            return Option<DateTime>.None();

        int year = DateTime.Now.Year;
        DateTime date = new DateTime(year, month.Value, day.Value);
        return Option<DateTime>.Some(date);
    }

    private Option<int> GetMonthNumber()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            foreach (
                var date in dates.Where(date =>
                    word.StartsWith(date.Value, StringComparison.OrdinalIgnoreCase)
                )
            )
            {
                return Option<int>.Some(date.Key);
            }
        }

        return Option<int>.None();
    }

    private Option<int> GetDayBeforeMonth(int matchedMonth)
    {
        string monthText = dates[matchedMonth];
        string pattern = @$"(\d{{1,2}})\s+{monthText}";

        Match match = Regex.Match(_date, pattern, RegexOptions.IgnoreCase);

        if (!match.Success || !int.TryParse(match.Groups[1].Value, out int day))
            return Option<int>.None();
        if (day >= 1 && day <= DateTime.DaysInMonth(DateTime.Now.Year, matchedMonth))
            return Option<int>.Some(day);

        return Option<int>.None();
    }
}
