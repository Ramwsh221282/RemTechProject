using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.CatalogueParsing;
using Serilog;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CataloguePagination
{
    private readonly string _baseCatalogueUrl;
    private readonly IMessagePublisher _publisher;
    private readonly ILogger _logger;
    private int _maxPage = -1;
    private int _currentPage = -1;

    public CataloguePagination(string url, IMessagePublisher publisher, ILogger logger)
    {
        _baseCatalogueUrl = url;
        _publisher = publisher;
        _logger = logger;
    }

    public void SetMaxPage(ref int value)
    {
        if (_maxPage != -1)
            return;
        _maxPage = value;
        _currentPage = 1;
    }

    public async Task InitializePagination(CancellationToken ct = default)
    {
        var initialization = new InitializePaginationBehavior(this, _logger);
        await initialization.Execute(_publisher, ct);
    }

    public IEnumerable<string> GetCataloguePageUrls()
    {
        while (_currentPage <= _maxPage)
        {
            ReadOnlySpan<char> url = _baseCatalogueUrl;
            var result = $"{url}?p={_currentPage}";
            _currentPage++;
            yield return result;
        }
    }
}
