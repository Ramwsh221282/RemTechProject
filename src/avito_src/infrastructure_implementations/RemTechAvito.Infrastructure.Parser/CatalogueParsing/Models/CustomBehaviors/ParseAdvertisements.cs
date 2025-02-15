using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class ParseAdvertisements : IWebDriverBehavior
{
    private readonly CataloguePageModel _model;

    public ParseAdvertisements(CataloguePageModel model) => _model = model;

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        foreach (var section in _model.ItemSections)
        {
            foreach (var item in section.Value)
            {
                OpenPageBehavior open = new OpenPageBehavior(item.Url);
                ScrollToBottomRetriable bottom = new ScrollToBottomRetriable(10);
                ScrollToTopRetriable top = new ScrollToTopRetriable(10);
                //ParseTitleBehavior title = new ParseTitleBehavior(item);

                await open.Execute(publisher, ct);
                await bottom.Execute(publisher, ct);
                await top.Execute(publisher, ct);
                //await title.Execute(publisher, ct);
            }
        }

        return Result.Success();
    }
}
