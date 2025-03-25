using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Endpoints.GetCharacteristicsEndpoint;

public sealed record CharacteristicResponse(string Name);

public sealed record GetCharacteristicsMessage() : IContract;

public sealed class GetCharacteristicsMessageHandler(
    CharacteristicsRepository repository,
    CharacteristicsCache cache
) : IContractHandler<GetCharacteristicsMessage>
{
    private readonly CharacteristicsRepository _repository = repository;
    private readonly CharacteristicsCache _cache = cache;

    public async Task<ContractActionResult> Handle(GetCharacteristicsMessage contract)
    {
        if (!_cache.IsEmpty)
        {
            CharacteristicResponse[] response =
            [
                .. _cache.Get().Select(i => new CharacteristicResponse(i.Name)),
            ];
            return ContractActionResult.Success(response);
        }

        CharacteristicResponse[] fromDb =
        [
            .. (await _repository.Get()).Select(i => new CharacteristicResponse(i.Name)),
        ];
        return ContractActionResult.Success(fromDb);
    }
}
