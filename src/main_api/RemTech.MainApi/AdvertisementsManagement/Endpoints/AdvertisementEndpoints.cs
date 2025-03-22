using Microsoft.AspNetCore.Mvc;
using RemTech.MainApi.AdvertisementsManagement.Dtos;
using RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.AdvertisementsManagement.Responses;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.Common.Dtos;
using RemTech.MainApi.Common.Envelope;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Endpoints;

[EndpointMapping]
public static class AdvertisementEndpoints
{
    [EndpointMappingMethod]
    public static void Map(this WebApplication app)
    {
        var group = app.MapGroup("/api/advertisements").RequireCors("frontend");
        group.MapPost("", GetWithFilters);
    }

    private static async Task<IResult> GetWithFilters(
        [FromServices]
            IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>> handler,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromBody] AdvertisementsFilterOption option,
        CancellationToken ct
    )
    {
        PaginationOption pagination = new(page, pageSize);
        GetAdvertisementsQuery query = new(option, pagination);
        Option<Result<TransportAdvertisement[]>> queryResult = await handler.Handle(query, ct);
        return !queryResult.HasValue ? Results.InternalServerError()
            : queryResult.Value.IsFailure ? queryResult.Value.Envelope()
            : Result<TransportAdvertisementResponse[]>
                .Success([.. queryResult.Value.Value.Select(r => r.ToResponse())])
                .Envelope();
    }
}
