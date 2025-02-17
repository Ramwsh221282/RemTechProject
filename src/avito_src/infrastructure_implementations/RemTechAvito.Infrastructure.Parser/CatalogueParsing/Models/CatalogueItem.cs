using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;

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
    public string[] PhotoLinks { get; set; } = Array.Empty<string>();
    public CatalogueItemPriceInfo Price { get; } = new CatalogueItemPriceInfo();

    public ParsedTransportAdvertisement ToParsed() =>
        new(
            Url,
            Id,
            Title,
            SellerInfo.ToParsed(),
            Characteristics,
            Address,
            Description,
            Date,
            PhotoLinks,
            Price.ToParsed()
        );
}

internal sealed class CatalogueItemPriceInfo
{
    public string Value { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;

    public ParsedTransportAdvertisementPriceInfo ToParsed() => new(Value, Currency, Extra);
}

internal sealed class SellerInfo
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public ParsedTransportAdvertisementSellerInfo ToParsed() => new(Name, Status);
}
