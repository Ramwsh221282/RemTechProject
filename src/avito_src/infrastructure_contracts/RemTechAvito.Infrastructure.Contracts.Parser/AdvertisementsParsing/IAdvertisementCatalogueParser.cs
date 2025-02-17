namespace RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;

public interface IAdvertisementCatalogueParser
{
    IAsyncEnumerable<ParsedTransportAdvertisement> Parse(
        string catalogueUrl,
        CancellationToken ct = default
    );
}
