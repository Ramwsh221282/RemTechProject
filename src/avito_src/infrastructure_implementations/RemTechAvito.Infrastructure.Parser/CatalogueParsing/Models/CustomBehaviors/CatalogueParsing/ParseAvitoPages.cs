using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.CatalogueParsing;

internal sealed class ParseAvitoPages(CataloguePageModel model, ILogger logger) : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        logger.Information("Starting fetching avito pages...");
        for (int index = 0; index < model.MaxPage; index++)
        {
            string url = model.NewPageUrl;
            OpenPageBehavior openPage = new OpenPageBehavior(url);
            await openPage.Execute(publisher, ct);

            ParseCataloguePageBehavior execution = new ParseCataloguePageBehavior(model, logger);
            await execution.Execute(publisher, ct);
            model.IncrementPage();
        }
        logger.Information(
            "Total page catalogues to parse {Number}",
            model.ItemSections.Last().Key
        );
        logger.Information("Total page items to parse {Number}", model.ItemSections.Count);
        return Result.Success();
    }
}
