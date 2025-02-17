using HtmlAgilityPack;
using RemTechAvito.Infrastructure.Parser.Extensions;
using Serilog;

namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.SellerInfoParsing;

internal sealed class CompanyParsingNode(
    HtmlDocument doc,
    CatalogueItem item,
    ILogger logger,
    ISellerInfoParsingChain? next = null
) : ISellerInfoParsingChain
{
    private const string path = ".//div[@data-marker='seller-info/name']";
    public ISellerInfoParsingChain? Next { get; } = next;

    public void TryParse()
    {
        logger.Information("{Action} attempt to parse as company.", nameof(CompanyParsingNode));
        try
        {
            HtmlNode? companyAttributesContainer = doc.DocumentNode.SelectSingleNode(path);
            if (companyAttributesContainer == null)
            {
                logger.Warning("{Action} not a company name.", nameof(CompanyParsingNode));
                Next?.TryParse();
                return;
            }

            HtmlNode companyAttributesGroup = companyAttributesContainer.FirstChild;
            HtmlNode companyAttributesWrapper = companyAttributesGroup.FirstChild;
            HtmlNode nameContainer = companyAttributesWrapper.FirstChild;
            HtmlNode statusContainer = companyAttributesWrapper.LastChild;

            item.SellerInfo.Name = nameContainer.InnerText.CleanString();
            item.SellerInfo.Status = statusContainer.InnerText.CleanString();
        }
        catch (Exception ex)
        {
            logger.Fatal("{Action} exception: {Message}", nameof(CompanyParsingNode), ex.Message);
            Next?.TryParse();
        }
    }
}
