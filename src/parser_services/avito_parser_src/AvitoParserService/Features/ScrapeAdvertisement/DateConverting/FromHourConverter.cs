using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public sealed class FromHourConverter : IDateConverter
{
    private const string sample = "час";
    private readonly string _date;

    public FromHourConverter(string date) => _date = date;

    public Option<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return Option<DateTime>.None();

        return !HasHourSample() ? Option<DateTime>.None() : Option<DateTime>.Some(DateTime.Now);
    }

    private bool HasHourSample()
    {
        ReadOnlySpan<string> words = _date.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (string word in words)
        {
            if (word.StartsWith(sample, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
