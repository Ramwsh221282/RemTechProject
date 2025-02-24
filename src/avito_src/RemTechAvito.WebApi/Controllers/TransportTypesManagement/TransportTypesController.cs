using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;

namespace RemTechAvito.WebApi.Controllers.TransportTypesManagement;

public sealed class TransportTypesController : ApplicationController
{
    [EndpointDescription("Returns transport types list based on parameters")]
    [EndpointSummary("Returns transport types")]
    [EndpointName("Transport Types Get")]
    [HttpGet("/transport-types")]
    public async Task<IActionResult> Get(
        [FromServices] ITransportTypesQueryRepository repository,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string? sort,
        [FromQuery] string? mark,
        CancellationToken ct = default
    )
    {
        GetTransportTypesQueryPagination pagination = new(page, size);
        var sorting = sort == null ? null : new GetTransportTypesQuerySortMode(sort);
        var search = mark == null ? null : new GetTransportTypesTextSearch(mark);
        var query = new GetTransportTypesQuery(pagination, sorting, search);
        var items = await repository.Get(query, ct);
        return this.ToOkResult(items);
    }
}
