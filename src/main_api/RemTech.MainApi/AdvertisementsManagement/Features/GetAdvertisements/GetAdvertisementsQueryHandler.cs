using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

public sealed class GetAdvertisementsQueryHandler(DataServiceMessager messager)
    : IQueryHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    >
{
    private readonly DataServiceMessager _messager = messager;

    public async Task<Option<Result<(TransportAdvertisement[] advertisements, long count)>>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        var payload = query.Option.FromFilterOption();
        var pagination = query.Pagination;
        AdvertisementsQuery dataServiceQuery = new(payload, pagination);
        GetAdvertisementsMessage message = new(dataServiceQuery);
        var result = await _messager.Send(message, ct);
        if (!result.IsSuccess)
            return Option<Result<(TransportAdvertisement[], long)>>.Some(new Error(result.Error));

        var response = result.FromResult<TransportAdvertisementDaoResponse>();
        var advertisements = response.Advertisements;
        var count = response.Count;
        return Option<Result<(TransportAdvertisement[], long)>>.Some((advertisements, count));
    }
}
