using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CataloguePagination
{
    private readonly ReadOnlyMemory<char> _urlMemory;

    private bool _isPaginationInited;

    public CataloguePagination(string url) => _urlMemory = url.AsMemory();

    public int CurrentPage { get; private set; } = 1;
    public int MaxPage { get; private set; }

    public string NewPageUrl => $"{_urlMemory}?p={CurrentPage}";
    public bool IsReachedMaxPage => CurrentPage == MaxPage;

    public Result IncrementCurrentPage()
    {
        if (IsReachedMaxPage)
            return new Error("Max page reached");
        CurrentPage++;
        return Result.Success();
    }

    public void SetMaxPage(int page)
    {
        if (_isPaginationInited)
            return;

        MaxPage = page;

        _isPaginationInited = true;
    }
}
