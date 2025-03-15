using RemTech.MainApi.ParsersManagement.Requests;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTech.MainApi.ParsersManagement.Services;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Endpoints;

public static class ParserEndpoints
{
    public static void MapParserEndpoints(this WebApplication app)
    {
        var parserGroup = app.MapGroup("/api/parsers");
        parserGroup.MapPost("{name}", Create);
        parserGroup.MapPatch("{name}", Update);
        parserGroup.MapDelete("{name}", Delete);
        parserGroup.MapGet("{name}", Get);
        parserGroup.MapGet("", GetAll);
    }

    private static async Task<IResult> Create(ParserManagementService service, string name)
    {
        Result<ParserResponse> creation = await service.CreateDefault(name);
        return creation.IsSuccess
            ? Results.Ok(creation.Value)
            : Results.BadRequest(creation.Error.Description);
    }

    private static async Task<IResult> Update(
        string name,
        UpdateParserRequest request,
        ParserManagementService service
    )
    {
        Result<ParserResponse> update = await service.Update(name, request);
        return update.IsSuccess
            ? Results.Ok(update.Value)
            : Results.BadRequest(update.Error.Description);
    }

    private static async Task<IResult> Delete(string name, ParserManagementService service)
    {
        Result delete = await service.Delete(name);
        return delete.IsSuccess ? Results.Ok() : Results.BadRequest(delete.Error.Description);
    }

    private static async Task<IResult> Get(string name, ParserManagementService service)
    {
        Result<ParserResponse> parser = await service.Get(name);
        return parser.IsSuccess
            ? Results.Ok(parser.Value)
            : Results.NotFound(parser.Error.Description);
    }

    private static async Task<IResult> GetAll(ParserManagementService service)
    {
        Result<IEnumerable<ParserResponse>> parsers = await service.GetAll();
        return parsers.IsSuccess
            ? Results.Ok(parsers.Value)
            : Results.BadRequest(parsers.Error.Description);
    }
}
