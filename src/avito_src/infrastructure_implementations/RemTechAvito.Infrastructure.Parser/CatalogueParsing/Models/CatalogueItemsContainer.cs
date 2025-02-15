namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CatalogueItemsContainer
{
    private readonly Dictionary<int, CatalogueItem[]> _itemSections = [];
    public IReadOnlyDictionary<int, CatalogueItem[]> ItemSections => _itemSections;

    public void AddCatalogueItems(int page, CatalogueItem[] items)
    {
        if (_itemSections.ContainsKey(page))
            return;
        _itemSections.Add(page, items);
    }
}
