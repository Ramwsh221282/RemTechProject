using System.Text.RegularExpressions;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;
using RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Shared;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public sealed partial class FromMonthWithYearConverter(string date) : IAvitoDateConverter
{
    private const string pattern = @"(\d{1,2})\s+([\p{L}]+)\s+(\d{4})";
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

    private static readonly Regex _regex = Regex();

    private readonly string _date = date;

    public Result<DateTime> Convert()
    {
        Result<(int day, int month, int year)> dateMetadataResult = GetDateMetadata();
        if (dateMetadataResult.IsFailure)
            return dateMetadataResult.Error;

        (int day, int month, int year) dateMetadata = dateMetadataResult.Value;
        return new DateTime(dateMetadata.year, dateMetadata.month, dateMetadata.day);
    }

    public Result<(int day, int month, int year)> GetDateMetadata()
    {
        Match match = _regex.Match(_date);
        if (!match.Success)
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromMonthWithYearConverter));

        string dayString = match.Groups[1].Value;
        string monthString = match.Groups[2].Value;
        string yearString = match.Groups[3].Value;

        KeyValuePair<int, string>? monthPair = dates.FirstOrDefault(d =>
            AvitoConvertersShared.IsStringStartsWith(monthString, d.Value)
        );

        if (monthPair == null)
            return AvitoDateConvertingErrors.NotMatchingPattern(nameof(FromMonthWithYearConverter));

        int year = int.Parse(yearString);
        int month = monthPair.Value.Key;
        int day = int.Parse(dayString);

        return (day, month, year);
    }

    [GeneratedRegex(pattern, RegexOptions.Compiled)]
    private static partial Regex Regex();
}
