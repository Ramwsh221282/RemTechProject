using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Dtos;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

public sealed class GetAdvertisementsQueryHandler(DataServiceMessager messager)
    : IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    >
{
    private readonly DataServiceMessager _messager = messager;

    public async Task<Result<(TransportAdvertisement[] advertisements, long count)>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        AdvertisementQueryPayload payload = query.Option.FromFilterOption();
        PaginationOption pagination = query.Pagination;
        SortingOption sorting = query.Sorting;
        PriceFilterCriteria priceOpt = query.PriceFilter;
        TextSearchOption textSearch = query.TextSearch;
        AdvertisementsQuery dataServiceQuery = new(payload, pagination, sorting, priceOpt, textSearch);

        GetAdvertisementsMessage message = new(dataServiceQuery);
        ContractActionResult result = await _messager.Send(message, ct);

        return ResultExtensions
            .When<(TransportAdvertisement[], long)>(!result.IsSuccess)
            .ApplyError(result.Error)
            .Process(() =>
            {
                var response = result.FromResult<TransportAdvertisementDaoResponse>();
                var advertisements = response.Advertisements;
                var count = response.Count;
                return (advertisements, count);
            });
    }
}
