using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class ParseAvitoPages : IWebDriverBehavior
{
    private readonly CataloguePageModel _model;
    private readonly ILogger _logger;

    public ParseAvitoPages(CataloguePageModel model, ILogger logger)
    {
        _model = model;
        _logger = logger;
    }

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        _logger.Information("Starting fetching avito pages...");
        while (!_model.IsReachedMaxPage)
        {
            Result increment = _model.IncrementPage();
            if (increment.IsFailure)
                break;

            string url = _model.NewPageUrl;
            OpenPageBehavior openPage = new OpenPageRepeatable(url, 10);
            Result opening = await openPage.Execute(publisher, ct);
            if (opening.IsFailure)
                break;

            _logger.Information("Navigation: {Url}", url);
            ParseCataloguePageBehavior execution = new ParseCataloguePageBehavior(_model, _logger);
            await execution.Execute(publisher, ct);
        }
        return Result.Success();
    }
}
