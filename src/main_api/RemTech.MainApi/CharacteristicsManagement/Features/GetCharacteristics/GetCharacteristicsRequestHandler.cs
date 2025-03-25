using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.CharacteristicsManagement.Responses;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.CharacteristicsManagement.Features.GetCharacteristics;

public sealed record GetCharacteristicsMessage() : IContract;

public sealed class GetCharacteristicsRequestHandler(DataServiceMessager messager)
    : IRequestHandler<GetCharacteristicsRequest, Option<CharacteristicResponse[]>>
{
    private readonly DataServiceMessager _messager = messager;

    public async Task<Option<CharacteristicResponse[]>> Handle(
        GetCharacteristicsRequest request,
        CancellationToken ct = default
    )
    {
        ContractActionResult result = await _messager.Send(new GetCharacteristicsMessage());
        if (!result.IsSuccess)
            return Option<CharacteristicResponse[]>.None();
        CharacteristicResponse[] items = result.FromResult<CharacteristicResponse[]>();
        return items.Length == 0
            ? Option<CharacteristicResponse[]>.None()
            : Option<CharacteristicResponse[]>.Some(items);
    }
}
