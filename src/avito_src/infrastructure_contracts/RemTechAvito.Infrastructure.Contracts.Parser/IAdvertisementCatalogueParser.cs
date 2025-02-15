using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

namespace RemTechAvito.Infrastructure.Contracts.Parser;

public interface IAdvertisementCatalogueParser
{
    Task<IReadOnlyCollection<TransportAdvertisement>> Parse(
        string catalogueUrl,
        CancellationToken ct = default
    );
}
