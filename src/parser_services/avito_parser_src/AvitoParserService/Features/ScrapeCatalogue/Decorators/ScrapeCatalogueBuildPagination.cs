using System.Text;
using PuppeteerSharp;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageCreation;
using ILogger = Serilog.ILogger;

namespace AvitoParserService.Features.ScrapeCatalogue.Decorators;

public sealed class ScrapeCatalogueBuildPagination : IScrapeAdvertisementsHandler
{
    private const string paginationContainerClass = "js-pages pagination-pagination-Oz4Ri";
    private readonly ScrapeCatalogueContext _context;
    private readonly ILogger _logger;
    private readonly IScrapeAdvertisementsHandler _handler;

    public ScrapeCatalogueBuildPagination(
        IScrapeAdvertisementsHandler handler,
        ScrapeCatalogueContext context,
        ILogger logger
    )
    {
        _handler = handler;
        _context = context;
        _logger = logger;
    }

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        _logger.Information(
            "{Context} Scrape pagination operation started.",
            nameof(ScrapeAdvertisementCommand)
        );
        IBrowser browser = _context.Browser.Value;
        IPageCreationStrategy strategy = new DomLoadPageCreationStrategy(browser);
        IPageBehavior scrollTop = new ScrollTopBehavior();
        IPageBehavior scrollBottom = new ScrollBottomBehavior();
        PageFactory factory = new PageFactory(strategy);
        IPage page = await factory.Create(command.Url);
        PageBehaviorExecutor executor = new PageBehaviorExecutor(page);
        await executor.Invoke(scrollBottom);
        await executor.Invoke(scrollTop);
        await using IElementHandle? element = await executor.Invoke(
            new GetElementWithSelectorFormat(paginationContainerClass)
        );
        if (element == null)
        {
            _logger.Error(
                "{Warning} Pagination element was not found. Maybe one page.",
                nameof(ScrapeAdvertisementCommand)
            );
            _context.AddPageUrl(command.Url);
            await _handler.Handle(command);
            return;
        }

        IElementHandle[] paginationButtons = await element.QuerySelectorAllAsync("li");
        int maxPageNumber = 1;
        ElementBehaviorExecutor elementBehaviorExecutor = new ElementBehaviorExecutor(null!);
        foreach (var button in paginationButtons)
        {
            await using (button)
            {
                elementBehaviorExecutor.SwapElement(button);
                IElementBehavior<string?> getText = new GetElementTextBehavior();
                string? textContent = await elementBehaviorExecutor.Invoke(getText);

                if (textContent == null)
                    continue;

                bool canParseInt = int.TryParse(textContent, out int number);
                if (!canParseInt)
                    continue;

                if (number > maxPageNumber)
                    maxPageNumber = number;
            }
        }

        RegisterPagesBasedOnMaxPageNumber(ref maxPageNumber, command.Url);
        _logger.Information(
            "{Context} Scrape pagination operation finished.",
            nameof(ScrapeAdvertisementCommand)
        );
        _logger.Information(
            "{Context} max avito pages: {Number}.",
            nameof(ScrapeAdvertisementCommand),
            maxPageNumber
        );
        await page.DisposeAsync();
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
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(basicUrl);
        stringBuilder.Append($"?p={pageNumber}");
        return stringBuilder.ToString();
    }
}
