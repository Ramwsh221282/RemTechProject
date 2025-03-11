using RemTechCommon.Utils.ResultPattern;

namespace CreateAdvertisementDatePlugin.DateConverters;

public sealed class FromHourConverter : IDateConverter
{
    private const string sample = "час";
    private readonly string _date;

    public FromHourConverter(string date) => _date = date;

    public Result<DateTime> ConvertToDateTime()
    {
        if (string.IsNullOrWhiteSpace(_date))
            return new Error("String is empty");

        if (!HasHourSample())
            return new Error("Can't convert from hour converter");

        return DateTime.Now;
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
