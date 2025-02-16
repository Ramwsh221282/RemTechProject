using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.PhotoParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors;

internal sealed class ParseAdvertisements(CataloguePageModel model, ILogger logger)
    : IWebDriverBehavior
{
    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        foreach (var section in model.ItemSections)
        {
            logger.Information("Scraping page section: {Number}", section.Key);
            foreach (var item in section.Value)
            {
                logger.Information("Scraping item: {Url}...", item.Url);

                OpenPageBehavior open = new OpenPageBehavior(item.Url);
                ScrollToBottomRetriable bottom = new ScrollToBottomRetriable(5);
                ScrollToTopRetriable top = new ScrollToTopRetriable(5);

                await open.Execute(publisher, ct);
                await bottom.Execute(publisher, ct);
                await top.Execute(publisher, ct);

                ParsePriceBehavior price = new ParsePriceBehavior(item, logger);
                ParseSellerInfoBehavior seller = new ParseSellerInfoBehavior(item, logger);
                ParseCharacteristics characteristics = new ParseCharacteristics(item, logger);
                ParseAddressBehavior address = new ParseAddressBehavior(item, logger);
                ParseDateBehavior date = new ParseDateBehavior(item, logger);
                PhotoClickingBehavior photos = new PhotoClickingBehavior(item, logger);

                await price.Execute(publisher, ct);
                await seller.Execute(publisher, ct);
                await characteristics.Execute(publisher, ct);
                await address.Execute(publisher, ct);
                await date.Execute(publisher, ct);
                await photos.Execute(publisher, ct);

                logger.Information("Item: {Url} has been scraped", item.Url);
            }
        }

        return Result.Success();
    }
}
