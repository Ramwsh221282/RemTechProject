namespace RemTechAvito.Infrastructure.Contracts.Parser;

public sealed record ParsedTransportAdvertisement(
    string Url,
    string Id,
    string Title,
    ParsedTransportAdvertisementSellerInfo SellerInfo,
    string[] Characteristics,
    string Address,
    string Description,
    string Date,
    string[] PhotoLinks,
    ParsedTransportAdvertisementPriceInfo PriceInfo
);

public sealed record ParsedTransportAdvertisementSellerInfo(string Name, string Status);

public sealed record ParsedTransportAdvertisementPriceInfo(
    string Value,
    string Currency,
    string Extra
);
