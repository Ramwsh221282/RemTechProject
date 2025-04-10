using System.Text.Json;
using PuppeteerSharp;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping.Decorators;

public sealed class ScrapeConcreteAdvertisementInitializerDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context
) : IScrapeConcreteAdvertisementHandler
{
    private const string Selector = "[data-drom-module]";
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;

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
            _context.ScriptElement = Option<JsonDocument>.Some(JsonDocument.Parse(innerHtml));
            break;
        }
        return await _handler.Handle(command);
    }
}
