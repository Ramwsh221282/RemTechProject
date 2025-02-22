using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;

namespace RemTechAvito.WebApi.Controllers.TransportAdvertisements;

public sealed class TransportAdvertisementsController : ApplicationController
{
    [EndpointDescription("Retreives paginated advertisements. Can be sorted and filtered.")]
    [EndpointSummary("List Advertisements")]
    [EndpointName("Advertisements")]
    [HttpPost(Name = "Advertisements")]
    public async Task<IActionResult> GetAdvertisements(
        [FromServices] ITransportAdvertisementsQueryRepository repository,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string? sort,
        [FromBody] FilterAdvertisementsDto? filters
    )
    {
        if (page == 0 || size == 0)
            return this.ToErrorResult(
                HttpStatusCode.BadRequest,
                "Page and page size both required"
            );

        GetAnalyticsItemsRequest request = new GetAnalyticsItemsRequest(page, size, filters, sort);
        var results = await repository.Query(request);
        return this.ToOkResult(results);
    }

    [EndpointDescription("Retreives advertisement aggregation statistics. (Count, Max, Min, Avg).")]
    [EndpointSummary("Advertisement aggregation statistics")]
    [EndpointName("Aggregation statistics")]
    [HttpPost("/statistics")]
    public async Task<IActionResult> GetStatistics(
        [FromServices] ITransportAdvertisementsQueryRepository repository,
        [FromBody] FilterAdvertisementsDto? filters
    )
    {
        GetAnalyticsStatsRequest request = new GetAnalyticsStatsRequest(filters);
        var result = await repository.Query(request);
        return this.ToOkResult(result);
    }
}
