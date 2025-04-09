namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Shared;

public static class AvitoConvertersShared
{
    private const StringComparison StringComparisonOptions = StringComparison.OrdinalIgnoreCase;

    public static bool IsStringStartsWith(string arg1, string arg2)
    {
        return arg1.StartsWith(arg2, StringComparisonOptions);
    }

    public static bool IsStringContains(string arg1, string arg2)
    {
        return arg1.Contains(arg2);
    }
}
