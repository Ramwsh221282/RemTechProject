using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Dtos;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;

public sealed class GetAdvertisementsQueryValidationDecorator(
    IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>> handler
) : IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>>
{
    private readonly IQueryHandler<
        GetAdvertisementsQuery,
        Result<TransportAdvertisement[]>
    > _handler = handler;

    public async Task<Option<Result<TransportAdvertisement[]>>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        PaginationOption pagination = query.Pagination;
        if (pagination.Page <= 0)
            return new Error("Invalid page option.").AsSome<Result<TransportAdvertisement[]>>();
        if (pagination.PageSize <= 0)
            return new Error("Invalid page size option.").AsSome<
                Result<TransportAdvertisement[]>
            >();
        return await _handler.Handle(query, ct);
    }
}
