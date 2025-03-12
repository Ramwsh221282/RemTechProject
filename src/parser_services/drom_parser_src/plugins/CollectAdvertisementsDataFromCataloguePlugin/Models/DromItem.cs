using PuppeteerSharp;

namespace CollectAdvertisementsDataFromCataloguePlugin.Models;

public sealed record DromItem : IAsyncDisposable
{
    public const string ClassSelector = "css-1f68fiz ea1vuk60";
    public IElementHandle Element { get; }

    private DromItem(IElementHandle element) => Element = element;

    public async ValueTask DisposeAsync() => await Element.DisposeAsync();
}
