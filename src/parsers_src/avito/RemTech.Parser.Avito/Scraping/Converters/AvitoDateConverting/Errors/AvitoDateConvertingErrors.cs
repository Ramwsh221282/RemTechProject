using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting.Errors;

public static class AvitoDateConvertingErrors
{
    public static Error StringWasNullOrEmpty(string converter)
    {
        string message = $"Строка конвертации даты в {converter} была пустой.";
        return new Error(message);
    }

    public static Error NotMatchingPattern(string converter)
    {
        string message = $"Строка конвертации не соответствует шаблону в {converter}";
        return new Error(message);
    }
}
