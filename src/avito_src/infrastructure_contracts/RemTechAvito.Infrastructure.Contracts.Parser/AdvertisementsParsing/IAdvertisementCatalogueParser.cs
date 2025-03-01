namespace RemTechAvito.Infrastructure.Contracts.Parser.AdvertisementsParsing;

public interface IAdvertisementCatalogueParser
{
    IAsyncEnumerable<ParsedTransportAdvertisement> Parse(
        string catalogueUrl,
        IEnumerable<string>? additions = null,
        IEnumerable<long>? existingIds = null,
        CancellationToken ct = default
    );
}
