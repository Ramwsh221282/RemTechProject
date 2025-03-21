using PuppeteerSharp;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;

namespace DromParserService.Features.ScrapeConcreteAdvertisement.Decorators;

public sealed class ScrapeCharacteristicsDecorator(
    IScrapeConcreteAdvertisementHandler handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : IScrapeConcreteAdvertisementHandler
{
    private readonly Serilog.ILogger _logger = logger;
    private readonly IScrapeConcreteAdvertisementHandler _handler = handler;
    private readonly ScrapeConcreteAdvertisementContext _context = context;
    private const string _mainContainer = "css-0 epjhnwz1";
    private const string _tableSelector = "css-xalqz7 eo7fo180";

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        IPage page = _context.Page.Value;
        Option<IElementHandle> ctxContainer = await page.GetElementWithClassFormatter(
            _mainContainer
        );
        if (!ctxContainer.HasValue)
        {
            _logger.Error(
                "{Context} no characteristics found. Url: {Url}",
                nameof(ScrapeCharacteristicsDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle container = ctxContainer.Value;
        Option<IElementHandle> table = await container.GetChildWithClassFormatter(_tableSelector);
        if (!table.HasValue)
        {
            _logger.Error(
                "{Context} no characteristics found. Url: {Url}",
                nameof(ScrapeCharacteristicsDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle tableElement = table.Value;
        Option<IElementHandle> tableBody = await tableElement.GetChildWithoutClassFormatter(
            string.Intern("tbody")
        );
        if (!tableBody.HasValue)
        {
            _logger.Error(
                "{Context} no characteristics found. Url: {Url}",
                nameof(ScrapeCharacteristicsDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        await using IElementHandle tableBodyValue = tableBody.Value;
        Option<IElementHandle[]> tableRows = await tableBodyValue.GetChildrenWithoutClassFormatter(
            string.Intern("tr")
        );
        if (!tableRows.HasValue)
        {
            _logger.Error(
                "{Context} no characteristics found. Url: {Url}",
                nameof(ScrapeCharacteristicsDecorator),
                _context.Advertisement.Value.SourceUrl
            );
            return Option<ScrapedAdvertisement>.None();
        }

        IElementHandle[] tableRowsValue = tableRows.Value;
        List<ScrapedCharacteristic> ctx = [];
        foreach (var tr in tableRowsValue)
        {
            Option<IElementHandle> ctxNameElement = await tr.GetChildWithoutClassFormatter(
                string.Intern("th")
            );
            Option<IElementHandle> ctxValueElement = await tr.GetChildWithoutClassFormatter(
                string.Intern("td")
            );
            if (!ctxNameElement.HasValue || !ctxValueElement.HasValue)
                continue;
            Option<string> ctxNameText = await ctxNameElement.Value.GetElementText();
            Option<string> ctxValueText = await ctxValueElement.Value.GetElementText();

            if (!ctxNameText.HasValue || !ctxValueText.HasValue)
            {
                await ctxNameElement.Value.DisposeAsync();
                await ctxValueElement.Value.DisposeAsync();
                continue;
            }

            string name = ctxNameText.Value;
            string value = ctxValueText.Value;

            if (value.Contains('*'))
                value = value.Replace(string.Intern("*"), string.Empty);
            name = name.CleanString();
            value = value.CleanString();
            ctx.Add(new ScrapedCharacteristic { Name = name, Value = value });
            await ctxNameElement.Value.DisposeAsync();
            await ctxValueElement.Value.DisposeAsync();
        }
        ScrapedCharacteristic[] existing = _context.Advertisement.Value.Characteristics;
        ScrapedCharacteristic[] updated = [.. existing, .. ctx];
        _context.Advertisement.Value.Characteristics = updated;
        return await _handler.Handle(command);
    }
}
