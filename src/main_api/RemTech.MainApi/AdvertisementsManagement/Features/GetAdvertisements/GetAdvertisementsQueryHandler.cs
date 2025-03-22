using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Dtos;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

public sealed class GetAdvertisementsQueryHandler(DataServiceMessager messager)
    : IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>>
{
    private readonly DataServiceMessager _messager = messager;

    public async Task<Option<Result<TransportAdvertisement[]>>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        AdvertisementQueryPayload payload = query.Option.FromFilterOption();
        PaginationOption pagination = query.Pagination;
        AdvertisementsQuery dataServiceQuery = new(payload, pagination);
        GetAdvertisementsMessage message = new(dataServiceQuery);
        ContractActionResult response = await _messager.Send(message, ct);
        return !response.IsSuccess
            ? Option<Result<TransportAdvertisement[]>>.Some(new Error(response.Error))
            : Option<Result<TransportAdvertisement[]>>.Some(
                response.FromResult<TransportAdvertisement[]>()
            );
    }
}
