using PuppeteerSharp;

namespace CollectAdvertisementsDataFromCataloguePlugin.Models;

public sealed record DromItemsContainer : IAsyncDisposable
{
    public const string ClassSelector = "div[data-bulletin-list='true']";
    public IElementHandle Element { get; }

    private DromItemsContainer(IElementHandle element) => Element = element;

    public async ValueTask DisposeAsync() => await Element.DisposeAsync();
}
