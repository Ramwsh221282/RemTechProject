using System.Text.Json;
using RemTechCommon.Utils.OptionPattern;

namespace DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;

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
