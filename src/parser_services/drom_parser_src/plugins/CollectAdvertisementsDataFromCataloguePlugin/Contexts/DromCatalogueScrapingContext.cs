using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using CollectAdvertisementsDataFromCataloguePlugin.Extensions;
using CollectAdvertisementsDataFromCataloguePlugin.Models;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CollectAdvertisementsDataFromCataloguePlugin.Contexts;

public sealed class DromCatalogueScrapingContext
{
    private readonly IBrowser _browser;
    private readonly PluginExecutionContext _context;
    private readonly ScrapedSourceUrl _baseUrl;
    private readonly ILogger _logger;
    private int _currentPage = 1;

    public DromCatalogueScrapingContext(
        ILogger logger,
        IBrowser browser,
        PluginExecutionContext context,
        ScrapedSourceUrl baseUrl
    ) => (_browser, _context, _baseUrl, _logger) = (browser, context, baseUrl, logger);

    public async Task<List<ScrapedAdvertisement>> Process()
    {
        List<ScrapedAdvertisement> _collected = [];
        while (true)
        {
            await using DromCataloguePage cataloguePage = await DromCataloguePage.Create(
                _browser,
                _baseUrl.CreatePageUrl(_currentPage)
            );
            await cataloguePage.ScrollPageDown(_context, _logger);
            await cataloguePage.ScrollPageUp(_context, _logger);
            Result<DromItemsContainer> extractContainer =
                await cataloguePage.CreateItemsContainer();

            if (extractContainer.IsFailure)
            {
                _logger.Information(
                    "{Context} reached page without items",
                    nameof(DromCatalogueScrapingContext)
                );
                break;
            }

            DromItemsContainer container = extractContainer.Value;
            await using ItemsContainerContext itemsContext = new ItemsContainerContext(
                container,
                _logger
            );
            _collected.AddRange(await itemsContext.CreateAdvertisements());
            _currentPage++;
        }

        return _collected;
    }
}
