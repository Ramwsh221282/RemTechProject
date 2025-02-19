using HtmlAgilityPack;
using RemTechAvito.Infrastructure.Parser.Extensions;
using Serilog;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.SellerInfoParsing;

internal sealed class PersonParsingNode(
    HtmlDocument doc,
    CatalogueItem item,
    ILogger logger,
    ISellerInfoParsingChain? next = null
) : ISellerInfoParsingChain
{
    private const string namePath = ".//div[@data-marker='seller-info/name']";
    private const string statusPath = ".//div[@data-marker='seller-info/label']";
    public ISellerInfoParsingChain? Next { get; } = next;

    public void TryParse()
    {
        logger.Information("{Action} attempt to parse as person", nameof(PersonParsingNode));
        try
        {
            HtmlNode? sellerNameNode = doc.DocumentNode.SelectSingleNode(namePath);
            if (sellerNameNode == null)
            {
                logger.Warning("{Action} not a person", nameof(PersonParsingNode));
                Next?.TryParse();
                return;
            }

            HtmlNode? sellerStatusNode = doc.DocumentNode.SelectSingleNode(statusPath);
            if (sellerStatusNode == null)
            {
                logger.Warning("{Action} not a person", nameof(PersonParsingNode));
                Next?.TryParse();
                return;
            }

            item.SellerInfo.Name = sellerNameNode.InnerText.CleanString();
            item.SellerInfo.Status = "Частное лицо";
        }
        catch (Exception ex)
        {
            logger.Fatal("{Action} exception: {Message}", nameof(CompanyParsingNode), ex.Message);
            Next?.TryParse();
        }
    }
}
