using AvitoParser.PDK.Models;
using CollectAdvertisementsDataFromCataloguePlugin.Extensions;
using CollectAdvertisementsDataFromCataloguePlugin.Models;
using Serilog;

namespace CollectAdvertisementsDataFromCataloguePlugin.Contexts;

public sealed class ItemsContainerContext : IAsyncDisposable
{
    private readonly DromItemsContainer _container;
    private readonly ILogger _logger;

    public ItemsContainerContext(DromItemsContainer container, ILogger logger) =>
        (_container, _logger) = (container, logger);

    public async Task<ScrapedAdvertisement[]> CreateAdvertisements()
    {
        DromItem[] dromItems = await _container.ExtractAdvertisementsFromCatalogue();
        ScrapedAdvertisement[] advertisements = new ScrapedAdvertisement[dromItems.Length];

        _logger.Information(
            "{Context} created {Count} items.",
            nameof(ItemsContainerContext),
            dromItems.Length
        );

        for (int i = 0; i < dromItems.Length; i++)
        {
            await using (dromItems[i])
            {
                advertisements[i] = new ScrapedAdvertisement();
                advertisements[i] = await dromItems[i].UpdateWithTitleInfo(advertisements[i]);
                advertisements[i] = advertisements[i].UpdateWithCharacteristics();
                advertisements[i] = await dromItems[i].UpdateWithPhotos(advertisements[i]);
            }
        }

        return advertisements;
    }

    public async ValueTask DisposeAsync() => await _container.DisposeAsync();
}
