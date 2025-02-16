using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.CatalogueParsing;

internal sealed class InitializePaginationBehavior : IWebDriverBehavior
{
    private readonly CataloguePagination _pagination;
    private readonly ILogger _logger;
    private readonly string _baseUrl;
    private const string pathType = "xpath";
    private const string paginationPath = ".//ul[@data-marker='pagination-button']";
    private const string pagination = "pagination";

    public InitializePaginationBehavior(
        CataloguePagination pagination,
        ILogger logger,
        string baseUrl
    )
    {
        _pagination = pagination;
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
            .AddBehavior(new ClearPoolBehavior());

        Result execution = await behavior.Execute(publisher, ct);
        if (execution.IsFailure)
            return execution.Error;

        Result<WebElement> paginationContainer = pool[^1];
        if (paginationContainer.IsFailure)
        {
            InitializeOnePageOnly();
            return Result.Success();
        }

        HtmlNode paginationNode = HtmlNode.CreateNode(paginationContainer.Value.OuterHTML);
        HtmlNodeCollection? pages = paginationNode.SelectNodes(".//li");
        if (pages == null)
        {
            InitializeOnePageOnly();
            return Result.Success();
        }

        IEnumerable<HtmlNode> reversed = pages.Reverse();
        foreach (var node in reversed)
        {
            if (!int.TryParse(node.InnerText, out int page))
                continue;
            _pagination.SetMaxPage(ref page);
            _logger.Information("Initialized Max Page Numbers: {Count}", page);
            break;
        }

        return Result.Success();
    }

    private void InitializeOnePageOnly()
    {
        int page = 1;
        _pagination.SetMaxPage(ref page);
        _logger.Information("Pool empty. Initialized Max Page Numbers: {Count}", 1);
    }
}
