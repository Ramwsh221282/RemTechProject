namespace RemTechAvito.Infrastructure.Parser.CatalogueParsing.Models;

internal sealed class CatalogueItem
{
    public string Url { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public SellerInfo SellerInfo { get; } = new SellerInfo();
    public string[] Characteristics { get; set; } = Array.Empty<string>();
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public CatalogueItemPriceInfo Price { get; } = new CatalogueItemPriceInfo();
}

internal sealed class CatalogueItemPriceInfo
{
    public string Value { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
}

internal sealed class SellerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
