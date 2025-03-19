using RemTechCommon.Utils.OptionPattern;

namespace AvitoParserService.Features.ScrapeAdvertisement.DateConverting;

public sealed class FromTodayConverter : IDateConverter
{
    private const string sample = "сегодн";
    private readonly string _date;

    public FromTodayConverter(string date)
    {
        _date = date;
    }

    public Option<DateTime> Convert()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return Option<DateTime>.None();
        string[] parts = _date.Split(' ');
        return parts.Any(p => p.StartsWith(sample, StringComparison.OrdinalIgnoreCase))
            ? Option<DateTime>.Some(DateTime.Now)
            : Option<DateTime>.None();
    }
}
