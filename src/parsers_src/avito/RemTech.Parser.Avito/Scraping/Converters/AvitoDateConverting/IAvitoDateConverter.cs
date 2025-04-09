using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Parser.Avito.Scraping.Converters.AvitoDateConverting;

public interface IAvitoDateConverter
{
    Result<DateTime> Convert();
}
