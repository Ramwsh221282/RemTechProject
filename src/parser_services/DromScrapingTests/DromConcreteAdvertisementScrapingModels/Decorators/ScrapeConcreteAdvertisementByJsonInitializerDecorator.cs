using System.Text.Json;
using DromParserService.Features.DromCatalogueScraping.ConcreteAdvertisementScraping;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;

namespace DromScrapingTests.DromConcreteAdvertisementScrapingModels.Decorators;

public sealed class ScrapeConcreteAdvertisementByJsonInitializerDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementByJsonContext byJsonContext
) : IScrapeConcreteAdvertisementHandler
{
    private const string Selector = "[data-drom-module]";
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementByJsonContext _byJsonContext = byJsonContext;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = command.Page;
        await page.WaitForSelectorAsync(Selector);
        IElementHandle[] elements = await page.QuerySelectorAllAsync(Selector);
        foreach (IElementHandle item in elements)
        {
            string innerHtml = await item.EvaluateFunctionAsync<string>("el => el.innerHTML");
            if (!innerHtml.Contains("header", StringComparison.OrdinalIgnoreCase))
                continue;
            _byJsonContext.ScriptElement = Option<JsonDocument>.Some(JsonDocument.Parse(innerHtml));
            break;
        }
        return await _handler.Handle(command);
    }
}
