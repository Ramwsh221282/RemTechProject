using System.Text.Json;
using RemTechCommon.Utils.OptionPattern;

namespace DromScrapingTests.DromConcreteAdvertisementScrapingModels;

public sealed class ScrapeConcreteAdvertisementByJsonContext
{
    public Option<JsonDocument> ScriptElement { get; set; } = Option<JsonDocument>.None();

    public string PriceExtra { get; set; } = string.Empty;

    public void DisposeDocument()
    {
        if (ScriptElement.HasValue)
            ScriptElement.Value.Dispose();
    }
}
