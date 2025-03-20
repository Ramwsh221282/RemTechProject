using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public sealed class FromWeekConverter : IDateConverter
{
    private const string WeekSample = "недел";
    private readonly string _date;

    public FromWeekConverter(string date) => _date = date;

    public Option<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return Option<DateTime>.None();
        if (!HasWeekSample())
            return Option<DateTime>.None();
        DateTime date = GetDateWithWeekDifference();
        return Option<DateTime>.Some(date);
    }

    private bool HasWeekSample()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (word.StartsWith(WeekSample, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private DateTime GetDateWithWeekDifference()
    {
        int difference = _date.GetIntegerFromString() * 7;
        return DateTime.Now.AddDays(-difference);
    }
}
