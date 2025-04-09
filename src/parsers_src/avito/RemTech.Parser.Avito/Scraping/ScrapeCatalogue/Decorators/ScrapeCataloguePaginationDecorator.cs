using System.Text;
using PuppeteerSharp;
using RemTech.Parser.Avito.Scraping.Authorization;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Puppeteer.BrowserCreation;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Extensions;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue.Decorators;

public sealed class ScrapeCataloguePaginationDecorator(
    IScrapeAdvertisementsHandler handler,
    ScrapeCatalogueContext context,
    Serilog.ILogger logger
) : IScrapeAdvertisementsHandler
{
    private const string paginationSelector = "nav[aria-label='Пагинация']";
    private readonly IScrapeAdvertisementsHandler _handler = handler;
    private readonly ScrapeCatalogueContext _context = context;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        _logger.Information("{Action} started", nameof(ScrapeCataloguePaginationDecorator));

        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance(false);
        AvitoAuthorization authorization = new AvitoAuthorization(browser);

        Result authorizationResult = await authorization.Authorize();
        if (authorizationResult.IsFailure)
        {
            _logger.Error(
                "{Action} unable to authorize.",
                nameof(ScrapeCataloguePaginationDecorator)
            );
            await _handler.Handle(command);
            return;
        }

        IPage page = await browser.CreateByDomLoadNoImages(command.Url);

        await page.ScrollBottom();

        Option<IElementHandle> paginationContainer = await page.GetElementWithoutClassFormatter(
            paginationSelector
        );

        if (paginationContainer.HasValue == false)
        {
            _logger.Warning("No pagination element detected. Probably one page.");
            await _handler.Handle(command);
            return;
        }

        Option<IElementHandle[]> paginationButtons =
            await paginationContainer.Value.GetChildrenWithoutClassFormatter("li");
        if (paginationButtons.HasValue == false)
        {
            _logger.Warning("No pagination buttons detected. Probably one page.");
            await _handler.Handle(command);
            return;
        }

        int maxPageNumber = 1;
        foreach (IElementHandle pageButton in paginationButtons.Value)
        {
            Option<string> pageText = await pageButton.GetElementText();
            if (pageText.HasValue == false)
                continue;

            if (!int.TryParse(pageText.Value, out int pageNumber))
                continue;

            if (maxPageNumber < pageNumber)
                maxPageNumber = pageNumber;
        }

        RegisterPagesBasedOnMaxPageNumber(ref maxPageNumber, command.Url);

        _logger.Information("Avito pagination max page number {Number}", maxPageNumber);

        page.Dispose();
        browser.Dispose();

        _logger.Information("{Action} finished", nameof(ScrapeCataloguePaginationDecorator));
        await _handler.Handle(command);
    }

    private void RegisterPagesBasedOnMaxPageNumber(ref int maxPageNumber, string basicUrl)
    {
        for (int i = 1; i <= maxPageNumber; i++)
        {
            string page = CreateNextPageString(ref i, basicUrl);
            _context.AddPageUrl(page);
        }
    }

    private static string CreateNextPageString(ref int pageNumber, string basicUrl)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(basicUrl);
        stringBuilder.Append($"?p={pageNumber}");
        return stringBuilder.ToString();
    }
}
