using Microsoft.AspNetCore.Mvc;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisementById;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements;
using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetCharacteristics;
using RemTech.Infrastructure.PostgreSql.Shared;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.WebApi.AdvertisementsContext.Responses;
using RemTech.WebApi.Configuration.Endpoints;
using RemTech.WebApi.Envelope;

namespace RemTech.WebApi.AdvertisementsContext.Endpoints;

[EndpointClass]
public static class AdvertisementEndpoints
{
    [EndpointMappingMethod]
    public static void MapParserEndpoints(this WebApplication app)
    {
        RouteGroupBuilder group = app.MapGroup("api/advertisements").RequireCors("frontend");
        group.MapGet("{id:long}", GetById);
        group.MapPost(string.Empty, GetCustomized);
        group.MapGet("characteristics", GetCharacteristics);
    }

    private static async Task<IResult> GetById(
        [FromRoute] long id,
        [FromServices] IQueryHandler<GetAdvertisementByIdQuery, Option<AdvertisementDao>> handler,
        CancellationToken ct
    )
    {
        GetAdvertisementByIdQuery query = new(id);
        Option<AdvertisementDao> advertisementDao = await handler.Handle(query, ct);

        if (advertisementDao.HasValue == false)
            return UnitResult<AdvertisementResponse>
                .FromFailure(new Error("Объявление не найдено."), UnitResultCodes.NotFound)
                .AsEnvelope();

        return UnitResult<AdvertisementResponse>
            .FromSuccess(advertisementDao.Value.FromDao())
            .AsEnvelope();
    }

    private static async Task<IResult> GetCustomized(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? sort,
        [FromBody] GetAdvertisementsQueryDto? filterData,
        [FromServices]
            IQueryHandler<GetAdvertisementsQuery, PaginatedDaoResponse<AdvertisementDao>> handler,
        CancellationToken ct
    )
    {
        PaginationOptions pagination = new(page, pageSize);
        SortOptions? sortMode = sort == null ? null : new SortOptions(sort);
        FilterOptions? filter = filterData?.Filter;

        GetAdvertisementsQuery query = new(pagination, filter, sortMode);
        PaginatedDaoResponse<AdvertisementDao> response = await handler.Handle(query, ct);
        AdvertisementPaginatedResponse result = AdvertisementPaginatedResponse.Create(response);

        return UnitResult<AdvertisementPaginatedResponse>.FromSuccess(result).AsEnvelope();
    }

    private static async Task<IResult> GetCharacteristics(
        [FromServices] IQueryHandler<GetCharacteristicsQuery, TransportCharacteristicDao[]> handler,
        CancellationToken ct
    )
    {
        GetCharacteristicsQuery query = new();
        TransportCharacteristicDao[] data = await handler.Handle(query, ct);

        TransportCharacteristicResponse[] response = [.. data.Select(d => d.ToResponse())];
        return UnitResult<TransportCharacteristicResponse[]>.FromSuccess(response).AsEnvelope();
    }
}
