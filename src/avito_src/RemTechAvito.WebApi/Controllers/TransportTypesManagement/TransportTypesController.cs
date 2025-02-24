using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;
using RemTechCommon.Utils.ResultPattern;

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

    [EndpointDescription("Creates transport types collection in system")]
    [EndpointSummary("Creates transport types collection")]
    [EndpointName("Transport Types Create")]
    [HttpPost("/transport-types")]
    public async Task<IActionResult> Create(
        [FromServices] IAvitoCommandHandler<ParseTransportTypesCommand> handler,
        CancellationToken ct = default
    )
    {
        var command = new ParseTransportTypesCommand();
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? this.ToErrorResult(HttpStatusCode.InternalServerError, result.Error.Description)
            : this.ToOkResult();
    }
}
