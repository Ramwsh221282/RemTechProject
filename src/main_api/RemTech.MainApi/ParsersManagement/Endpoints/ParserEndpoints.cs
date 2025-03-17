using Microsoft.AspNetCore.Mvc;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Features.CreateParser;
using RemTech.MainApi.ParsersManagement.Features.DeleteParser;
using RemTech.MainApi.ParsersManagement.Features.GetAllParsers;
using RemTech.MainApi.ParsersManagement.Features.GetParser;
using RemTech.MainApi.ParsersManagement.Features.UpdateParser;
using RemTech.MainApi.ParsersManagement.Models;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Endpoints;

[EndpointMapping]
public static class ParserEndpoints
{
    [EndpointMappingMethod]
    public static void MapParserEndpoints(this WebApplication app)
    {
        var parserGroup = app.MapGroup("/api/parsers");
        parserGroup.MapPost("{name}", Create);
        parserGroup.MapPatch("{name}", Update);
        parserGroup.MapDelete("{name}", Delete);
        parserGroup.MapGet("{name}", Get);
        parserGroup.MapGet("", GetAll);
    }

    private static async Task<IResult> Create(
        [FromRoute] string name,
        [FromServices] ICommandHandler<CreateParserCommand, Parser> handler,
        CancellationToken ct
    )
    {
        CreateParserCommand command = new CreateParserCommand(name);
        Result<Parser> result = await handler.Handle(command, ct);
        return result.IsSuccess switch
        {
            true => Results.Ok(result.Value.ToResponse()),
            false => Results.BadRequest(result.Error),
        };
    }

    private static async Task<IResult> Delete(
        [FromRoute] string name,
        [FromServices] ICommandHandler<DeleteParserCommand, Result> handler,
        CancellationToken ct
    )
    {
        DeleteParserCommand command = new DeleteParserCommand(name);
        Result result = await handler.Handle(command, ct);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error.Description);
    }

    private static async Task<IResult> Update(
        [FromRoute] string name,
        [FromBody] ParserDto dto,
        [FromServices] ICommandHandler<UpdateParserCommand, ParserResponse> handler,
        CancellationToken ct
    )
    {
        ParserDto payload = dto with { ParserName = name };
        UpdateParserCommand command = new UpdateParserCommand(payload);
        Result<ParserResponse> result = await handler.Handle(command, ct);
        return result.IsSuccess ? Results.Ok() : Results.BadRequest(result.Error.Description);
    }

    private static async Task<IResult> Get(
        [FromRoute] string name,
        [FromServices] IQueryHandler<GetParserQuery, ParserResponse> handler,
        CancellationToken ct
    )
    {
        GetParserQuery query = new GetParserQuery(name);
        Option<ParserResponse> parser = await handler.Handle(query, ct);
        return parser.HasValue switch
        {
            true => Results.Ok(parser.Value),
            false => Results.NotFound(),
        };
    }

    private static async Task<IResult> GetAll(
        [FromServices] IQueryHandler<GetAllParsersQuery, ParserResponse[]> handler,
        CancellationToken ct
    )
    {
        Option<ParserResponse[]> option = await handler.Handle(new GetAllParsersQuery(), ct);
        return Results.Ok(option.Value);
    }
}
