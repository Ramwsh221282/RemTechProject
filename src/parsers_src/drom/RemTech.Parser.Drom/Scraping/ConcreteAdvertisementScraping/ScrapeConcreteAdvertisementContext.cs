using System.Text.Json;
using RemTech.Shared.SDK.OptionPattern;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public sealed class ScrapeConcreteAdvertisementContext
{
    public Option<JsonDocument> ScriptElement { get; set; } = Option<JsonDocument>.None();

    public string PriceExtra { get; set; } = string.Empty;

    public void DisposeDocument()
    {
        if (ScriptElement.HasValue)
            ScriptElement.Value.Dispose();
    }
}
