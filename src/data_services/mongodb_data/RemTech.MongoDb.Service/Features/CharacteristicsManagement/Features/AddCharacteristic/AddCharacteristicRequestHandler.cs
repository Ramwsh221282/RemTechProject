using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic;

public sealed class AddCharacteristicRequestHandler(
    CharacteristicsRepository repository,
    CharacteristicsCache cache
) : IRequestHandler<AddCharacteristicRequest, Result>
{
    private readonly CharacteristicsRepository _repository = repository;
    private readonly CharacteristicsCache _cache = cache;

    public async Task<Result> Handle(
        AddCharacteristicRequest request,
        CancellationToken ct = default
    )
    {
        await _repository.Save(request.Characteristic);
        _cache.Add(request.Characteristic);
        return Result.Success();
    }
}
