﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.ParserProfileManagement.CreateProfile;
using RemTechAvito.Application.ParserProfileManagement.DeleteProfile;
using RemTechAvito.Application.ParserProfileManagement.UpdateProfile;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;

namespace RemTechAvito.WebApi.Controllers.ParserProfileManagement;

public sealed class ParserProfileController : ApplicationController
{
    [EndpointDescription("Creates new parser profile. Returns new parser profile.")]
    [EndpointSummary("Creates new parser profile.")]
    [EndpointName("Parser Profile Create.")]
    [HttpPost("/parser-profile")]
    public async Task<IActionResult> Create(
        [FromServices] IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse> handler,
        [FromBody] CreateProfileCommand command,
        CancellationToken ct = default
    )
    {
        var result = await handler.Handle(command, ct);
        return result.IsSuccess
            ? this.ToOkResult(result.Value)
            : this.ToErrorResult(HttpStatusCode.BadRequest, result.Error.Description);
    }

    [EndpointDescription("Fully updates parser profile.")]
    [EndpointSummary("Updates parser profile.")]
    [EndpointName("Parser Profile Update.")]
    [HttpPut("/parser-profile/{id}")]
    public async Task<IActionResult> Update(
        [FromServices] IAvitoCommandHandler<UpdateParserProfileCommand> handler,
        [FromBody] ParserProfileDto dto,
        [FromRoute] string id,
        CancellationToken ct = default
    )
    {
        var command = new UpdateParserProfileCommand(dto);
        var result = await handler.Handle(command, ct);
        return result.IsSuccess
            ? this.ToOkResult()
            : this.ToErrorResult(HttpStatusCode.BadRequest, result.Error.Description);
    }

    [EndpointDescription("Deletes parser profile by id.")]
    [EndpointSummary("Deletes parser profile.")]
    [EndpointName("Parser Profile Delete.")]
    [HttpDelete("/parser-profile/{id}")]
    public async Task<IActionResult> Delete(
        [FromServices] IAvitoCommandHandler<DeleteParserProfileCommand> handler,
        [FromRoute] string id,
        CancellationToken ct = default
    )
    {
        var command = new DeleteParserProfileCommand(id);
        var result = await handler.Handle(command, ct);
        return result.IsSuccess
            ? this.ToOkResult()
            : this.ToErrorResult(HttpStatusCode.BadRequest, result.Error.Description);
    }

    [EndpointDescription("Returns parser profiles collection.")]
    [EndpointSummary("Returns all parser profiles.")]
    [EndpointName("Parser Profile Get")]
    [HttpGet("/parser-profile")]
    public async Task<IActionResult> Get(
        [FromServices] IParserProfileReadRepository repository,
        CancellationToken ct = default
    )
    {
        var result = await repository.Get(ct);
        return this.ToOkResult(result);
    }
}
