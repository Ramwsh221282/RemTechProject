using Microsoft.AspNetCore.Mvc;
using RemTech.Application.ParserContext.Dtos;
using RemTech.Application.ParserContext.Features.AddParserProfile;
using RemTech.Application.ParserContext.Features.RemoveParserProfile;
using RemTech.Application.ParserContext.Features.UpdateParserProfile;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetAllParsers;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.GetByName;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.DaoModels;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.WebApi.Configuration.Endpoints;
using RemTech.WebApi.Envelope;

namespace RemTech.WebApi.ParserContext.Endpoints;

[EndpointClass]
public static class ParserEndpoints
{
    [EndpointMappingMethod]
    public static void MapParserEndpoints(this WebApplication app)
    {
        RouteGroupBuilder parserGroup = app.MapGroup("/api/parsers").RequireCors("frontend");
        parserGroup.MapPost("{parserName}/{profileName}", AddProfile);
        parserGroup.MapDelete("{parserName}/{profileName}", RemoveProfile);
        parserGroup.MapPatch("{parserName}/{profileName}", UpdateProfile);
        parserGroup.MapGet("{parserName}", GetParserByName);
        parserGroup.MapGet(string.Empty, GetAllParsers);
    }

    private static async Task<IResult> AddProfile(
        [FromRoute] string parserName,
        [FromRoute] string profileName,
        [FromServices] ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> handler,
        CancellationToken ct
    )
    {
        AddParserProfileCommand command = new(parserName, profileName);
        UnitResult<Guid> result = await handler.Handle(command, ct);
        return result.AsEnvelope();
    }

    private static async Task<IResult> RemoveProfile(
        [FromRoute] string parserName,
        [FromRoute] string profileName,
        [FromServices] ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>> handler,
        CancellationToken ct
    )
    {
        RemoveParserProfileCommand command = new(parserName, profileName);
        UnitResult<Guid> result = await handler.Handle(command, ct);
        return result.AsEnvelope();
    }

    private static async Task<IResult> UpdateProfile(
        [FromRoute] string parserName,
        [FromRoute] string profileName,
        [FromBody] UpdateParserProfileDto updatedData,
        [FromServices] ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> handler,
        CancellationToken ct
    )
    {
        UpdateParserProfileCommand command = new(parserName, profileName, updatedData);
        UnitResult<Guid> result = await handler.Handle(command, ct);
        return result.AsEnvelope();
    }

    private static async Task<IResult> GetParserByName(
        [FromRoute] string parserName,
        [FromServices] IQueryHandler<GetParserByNameQuery, Option<ParserResponse>> handler
    )
    {
        GetParserByNameQuery query = new(parserName);
        Option<ParserResponse> response = await handler.Handle(query);
        return response.AsEnvelope();
    }

    private static async Task<IResult> GetAllParsers(
        [FromServices] IQueryHandler<GetAllParsersQuery, ParserResponse[]> handler
    )
    {
        GetAllParsersQuery query = new();
        ParserResponse[] response = await handler.Handle(query);
        return UnitResult<ParserResponse[]>.FromSuccess(response).AsEnvelope();
    }
}
