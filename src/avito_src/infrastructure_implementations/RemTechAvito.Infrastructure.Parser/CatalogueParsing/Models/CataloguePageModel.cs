using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CataloguePageModel
{
    private readonly CatalogueItemsContainer _container = new CatalogueItemsContainer();
    private readonly CataloguePagination _pagination;

    public CataloguePageModel(string baseUrl) => _pagination = new CataloguePagination(baseUrl);

    public void AddCatalogueItems(int pageNumber, CatalogueItem[] items) =>
        _container.AddCatalogueItems(pageNumber, items);

    public Result IncrementPage() => _pagination.IncrementCurrentPage();

    public void InitializePagination(int page) => _pagination.SetMaxPage(page);

    public IReadOnlyDictionary<int, CatalogueItem[]> ItemSections => _container.ItemSections;
    public bool IsReachedMaxPage => _pagination.IsReachedMaxPage;
    public string NewPageUrl => _pagination.NewPageUrl;
    public int CurrentPage => _pagination.CurrentPage;
    public int MaxPage => _pagination.MaxPage;
}
