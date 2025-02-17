using HtmlAgilityPack;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.SellerInfoParsing;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Worker.Service.Contracts.BaseImplementations;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;
using WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours.Implementations;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing;

internal sealed class ParseSellerInfoBehavior(CatalogueItem item, ILogger logger)
    : IWebDriverBehavior
{
    private const string pathType = "xpath";
    private const string sellerInfoPath = ".//div[@data-marker='item-view/seller-info']";

    public async Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default)
    {
        WebElementPool pool = new WebElementPool();
        GetNewElementRetriable getSellerInfo = new GetNewElementRetriable(
            pool,
            sellerInfoPath,
            pathType,
            "seller-info",
            5
        );
        ClearPoolBehavior clear = new ClearPoolBehavior();

        await getSellerInfo.Execute(publisher, ct);
        await clear.Execute(publisher, ct);

        InitializeSellerInfo(pool);
        return Result.Success();
    }

    private void InitializeSellerInfo(WebElementPool pool)
    {
        try
        {
            Result<WebElement> element = pool[^1];
            if (element.IsFailure)
            {
                logger.Error("{Action} cannot get seller info", nameof(ParseSellerInfoBehavior));
                return;
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(element.Value.OuterHTML);
            ISellerInfoParsingChain asPerson = new PersonParsingNode(doc, item, logger);
            ISellerInfoParsingChain asCompany = new CompanyParsingNode(doc, item, logger, asPerson);

            asCompany.TryParse();
            logger.Information(
                "Seller info: Name: {Name} Status: {Status}",
                item.SellerInfo.Name,
                item.SellerInfo.Status
            );
        }
        catch (Exception ex)
        {
            logger.Fatal("{Action}. Exception: {Ex}", nameof(ParseSellerInfoBehavior), ex.Message);
        }
    }
}
