namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models.CustomBehaviors.ConcreteAdvertisementParsing.SellerInfoParsing;

internal interface ISellerInfoParsingChain
{
    public ISellerInfoParsingChain? Next { get; }
    public void TryParse();
}
