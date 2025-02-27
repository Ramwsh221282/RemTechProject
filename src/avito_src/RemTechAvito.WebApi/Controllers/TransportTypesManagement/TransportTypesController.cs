using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.CreateCustomTransportType;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;

namespace RemTechAvito.WebApi.Controllers.TransportTypesManagement;

public sealed class TransportTypesController : ApplicationController
{
    [EndpointDescription("Returns transport types list based on parameters")]
    [EndpointSummary("Returns transport types")]
    [EndpointName("Transport Types Get")]
    [HttpGet("transport-types")]
    public async Task<IActionResult> Get(
        [FromServices] ITransportTypesQueryRepository repository,
        [FromQuery] int page,
        [FromQuery] int size,
        [FromQuery] string implementor,
        [FromQuery] string? sort,
        [FromQuery] string? mark,
        CancellationToken ct = default
    )
    {
        GetTransportTypesQueryPagination pagination = new(page, size);
        var sorting = sort == null ? null : new GetTransportTypesQuerySortMode(sort);
        var search = mark == null ? null : new GetTransportTypesTextSearch(mark);
        var implementorDto = new GetTransportTypesImplementor(implementor);
        var query = new GetTransportTypesQuery(pagination, implementorDto, sorting, search);
        var items = await repository.Get(query, ct);
        return this.ToOkResult(items);
    }

    [EndpointDescription("Creates system transport types collection using parser")]
    [EndpointSummary("Creates system transport types collection")]
    [EndpointName("System Transport Types Create")]
    [HttpPost("transport-types/parse")]
    public async Task<IActionResult> Create(
        [FromServices]
            IAvitoCommandHandler<ParseTransportTypesCommand, TransportTypeResponse> handler,
        CancellationToken ct = default
    )
    {
        var command = new ParseTransportTypesCommand();
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? this.ToErrorResult(HttpStatusCode.InternalServerError, result.Error.Description)
            : this.ToOkResult(result.Value);
    }

    [EndpointDescription("Creates user specific transport type in system")]
    [EndpointSummary("Creates user specific transport types")]
    [EndpointName("User Transport Types Create")]
    [HttpPost("transport-types/{name}")]
    public async Task<IActionResult> Create(
        [FromServices] IAvitoCommandHandler<CreateCustomTransportTypeCommand> handler,
        [FromBody] CreateCustomTransportTypeCommand command,
        CancellationToken ct = default
    )
    {
        var result = await handler.Handle(command, ct);
        return result.IsFailure
            ? this.ToErrorResult(HttpStatusCode.InternalServerError, result.Error.Description)
            : this.ToOkResult();
    }
}
