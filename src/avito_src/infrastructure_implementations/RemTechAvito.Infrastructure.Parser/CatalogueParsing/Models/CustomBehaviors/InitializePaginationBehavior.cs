using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class InitializePaginationBehavior : IWebDriverBehavior
{
    private readonly CataloguePageModel _model;
    private readonly ILogger _logger;
    private readonly string _baseUrl;
    private const string pathType = "xpath";
    private const string paginationPath = ".//ul[@data-marker='pagination-button']";
    private const string pagination = "pagination";

    public InitializePaginationBehavior(CataloguePageModel model, ILogger logger, string baseUrl)
    {
        _model = model;
        _logger = logger;
        _baseUrl = baseUrl;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        CompositeBehavior behavior = new CompositeBehavior()
            .AddBehavior(new OpenPageBehavior(_baseUrl))
            .AddBehavior(new ScrollToBottomRetriable(10))
            .AddBehavior(new ScrollToTopRetriable(10))
            .AddBehavior(new GetNewElementRetriable(pool, paginationPath, pathType, pagination, 10))
            .AddBehavior(new DoForLastParent(pool, element => new InitializeTextBehavior(element)));

        Result execution = await behavior.Execute(publisher, ct);
        if (execution.IsFailure)
            return execution.Error;

        Result<WebElement> element = pool[pool.Count - 1];
        if (element.IsFailure)
            return Result.Success();

        SetMaxPageNumber(element);

        _logger.Information("Initialized Max Page Numbers: {Count}", _model.MaxPage);

        return Result.Success();
    }

    private void SetMaxPageNumber(WebElement element)
    {
        ReadOnlySpan<char> text = element.Text;
        int lastNewLineSeparator = text.LastIndexOf('\n');

        ReadOnlySpan<char> lastNumber = text.Slice(lastNewLineSeparator + 1);
        if (int.TryParse(lastNumber, out int pageNumber))
            _model.InitializePagination(pageNumber);
    }
}
