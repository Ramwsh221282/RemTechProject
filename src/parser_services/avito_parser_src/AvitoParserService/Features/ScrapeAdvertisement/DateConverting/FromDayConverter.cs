using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public sealed class FromDayConverter : IDateConverter
{
    private static readonly string[] samples = ["ден", "дня", "дне"];
    private readonly string _date;

    public FromDayConverter(string date) => _date = date;

    public Option<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return Option<DateTime>.None();

        if (!HasAnySample())
            return Option<DateTime>.None();

        DateTime date = GetDateWithOneDayDifference();
        return Option<DateTime>.Some(date);
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
