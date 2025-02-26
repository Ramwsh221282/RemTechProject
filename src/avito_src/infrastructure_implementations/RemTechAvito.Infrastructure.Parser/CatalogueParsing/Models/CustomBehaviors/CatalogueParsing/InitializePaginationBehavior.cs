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
    private const string pathType = "xpath";
    private const string paginationPath = ".//ul[@data-marker='pagination-button']";
    private const string pagination = "pagination";

    public InitializePaginationBehavior(CataloguePagination pagination, ILogger logger)
    {
        _pagination = pagination;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        try
        {
            var pool = new WebElementPool();
            var getContainer = new GetNewElementRetriable(
                pool,
                paginationPath,
                pathType,
                pagination,
                5
            );

            var getContainerResult = await getContainer.Execute(publisher, ct);

            if (getContainerResult.IsFailure)
            {
                InitializeOnePageOnly();
                return Result.Success();
            }

            var paginationContainer = pool[^1];
            if (paginationContainer.IsFailure)
            {
                InitializeOnePageOnly();
                return Result.Success();
            }

            var paginationNode = HtmlNode.CreateNode(paginationContainer.Value.OuterHTML);
            var pages = paginationNode.SelectNodes(".//li");
            if (pages == null)
            {
                InitializeOnePageOnly();
                return Result.Success();
            }

            IEnumerable<HtmlNode> reversed = pages.Reverse();
            foreach (var node in reversed)
            {
                if (!int.TryParse(node.InnerText, out var page))
                    continue;
                _pagination.SetMaxPage(ref page);
                _logger.Information("Initialized Max Page Numbers: {Count}", page);
                break;
            }

            var clear = new ClearPoolBehavior();
            await clear.Execute(publisher, ct);
            return Result.Success();
        }
        catch
        {
            InitializeOnePageOnly();
            return Result.Success();
        }
    }

    private void InitializeOnePageOnly()
    {
        var page = 1;
        _pagination.SetMaxPage(ref page);
        _logger.Information("Pool empty. Initialized Max Page Numbers: {Count}", 1);
    }
}
