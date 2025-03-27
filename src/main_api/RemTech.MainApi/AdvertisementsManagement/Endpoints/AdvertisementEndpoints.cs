using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTech.MainApi.AdvertisementsManagement.Dtos;
using RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.AdvertisementsManagement.Responses;
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
        [FromServices] IRequestHandler<GetAdvertisementsQuery, Result<(TransportAdvertisement[] ads, long count)>> handler,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? sort,
        [FromQuery] string? priceStart,
        [FromQuery] string? priceEnd,
        [FromQuery] string? textSearchTerm,
        [FromBody] AdvertisementsFilterOption option,
        CancellationToken ct
    )
    {
        PaginationOption pagination = new(page, pageSize);
        SortingOption sortingOpt = string.IsNullOrWhiteSpace(sort)
            ? new SortingOption("NONE")
            : new SortingOption(sort);
        PriceFilterCriteria priceOpt = SpecifyCriteria(priceStart, priceEnd);
        TextSearchOption textSearchOpt = new TextSearchOption(textSearchTerm);
        
        GetAdvertisementsQuery query = new(option, pagination, sortingOpt, priceOpt, textSearchOpt);
        Option<Result<(TransportAdvertisement[], long)>> queryResult = await handler.Handle(query, ct);

        return queryResult.HasValue switch
        {
            false => EnvelopeResultExtensions.NotFound(),
            true when queryResult.Value.IsFailure => queryResult.Value.Envelope(),
            true when queryResult.Value.IsSuccess => EnvelopeResultExtensions.Envelope
                (
                    new TransportAdvertisementResponse(
                        [.. queryResult.Value.Value.Item1.Select(r => r.ToResponse())],
                        queryResult.Value.Value.Item2
                    )
                ),
            _ => EnvelopeResultExtensions.Envelope(HttpStatusCode.InternalServerError)
        };
    }

    private static PriceFilterCriteria SpecifyCriteria(string? priceStart, string? priceEnd) =>
        (priceStart, priceEnd) switch
        {
            (null, null) => new PriceFilterCriteria(0, 0, "NONE"),
            (not null, null) => new PriceFilterCriteria(
                long.TryParse(priceStart, out long value) ? value : 0,
                0,
                "MORE"
            ),
            (null, not null) => new PriceFilterCriteria(
                0,
                long.TryParse(priceEnd, out long value) ? value : 0,
                "LESS"
            ),
            (not null, not null)
                when long.TryParse(priceStart, out long valueA)
                    && long.TryParse(priceEnd, out long valueB) => new PriceFilterCriteria(
                valueA,
                valueB,
                "RANGE"
            ),
            _ => new PriceFilterCriteria(0, 0, "NONE"),
        };
}
