using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Dtos;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;

public sealed class GetAdvertisementsQueryValidationDecorator(
    IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > handler
)
    : IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    >
{
    private readonly IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > _handler = handler;

    public async Task<Result<(TransportAdvertisement[] advertisements, long count)>> Handle(
        GetAdvertisementsQuery request,
        CancellationToken ct = default
    )
    {
        PaginationOption pagination = request.Pagination;
        return await ResultExtensions
            .When<(TransportAdvertisement[], long)>(pagination.Page <= 0)
            .ApplyError("Invalid page option.")
            .AlsoWhen(pagination.PageSize <= 0)
            .ApplyError("Invalid page size option.")
            .Process(() => _handler.Handle(request, ct));
    }
}
