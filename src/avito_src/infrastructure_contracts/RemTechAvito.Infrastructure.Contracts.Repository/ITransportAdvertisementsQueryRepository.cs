using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;

namespace RemTechAvito.Infrastructure.Contracts.Repository;

public interface ITransportAdvertisementsQueryRepository
{
    Task<AdvertisementItemResponse[]> Query(
        GetAnalyticsItemsRequest query,
        CancellationToken ct = default
    );

    Task<AnalyticsStatsResponse> Query(
        GetAnalyticsStatsRequest query,
        CancellationToken ct = default
    );
}

public sealed record GetAnalyticsItemsRequest(
    int Page,
    int PageSize,
    FilterAdvertisementsDto? FilterData = null,
    string? SortOrder = null
);

public sealed record GetAnalyticsStatsRequest(FilterAdvertisementsDto? FilterData = null);

public sealed record AnalyticsStatsResponse(
    int Count,
    double AveragePrice,
    long MaxPrice,
    long MinPrice
);

public sealed class AdvertisementItemResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public long AdvertisementId { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string[] ImageLinks { get; set; } = Array.Empty<string>();
    public string SourceUrl { get; set; }
    public string Address { get; set; }
    public CharacteristicsResponse[] Characteristics { get; set; } =
        Array.Empty<CharacteristicsResponse>();
    public OwnerInformationResponse Owner { get; set; } = new OwnerInformationResponse();

    public PriceResponse Price { get; set; } = new PriceResponse();
}

public sealed class CharacteristicsResponse
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public sealed class OwnerInformationResponse
{
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class PriceResponse
{
    public long Value { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Extra { get; set; } = string.Empty;
}
