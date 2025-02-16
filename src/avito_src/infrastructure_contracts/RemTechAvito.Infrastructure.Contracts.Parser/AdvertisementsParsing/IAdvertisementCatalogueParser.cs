namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface IAdvertisementCatalogueParser
{
    IAsyncEnumerable<ParsedTransportAdvertisement> Parse(
        string catalogueUrl,
        CancellationToken ct = default
    );
}
