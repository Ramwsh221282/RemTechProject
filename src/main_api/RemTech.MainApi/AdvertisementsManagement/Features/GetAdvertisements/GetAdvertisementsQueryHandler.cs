using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.CqrsPattern;
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
        var payload = query.Option.FromFilterOption();
        var pagination = query.Pagination;
        AdvertisementsQuery dataServiceQuery = new(payload, pagination);
        GetAdvertisementsMessage message = new(dataServiceQuery);
        var result = await _messager.Send(message, ct);
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
