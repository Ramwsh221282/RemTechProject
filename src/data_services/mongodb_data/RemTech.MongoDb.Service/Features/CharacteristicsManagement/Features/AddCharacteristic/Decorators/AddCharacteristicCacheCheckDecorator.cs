using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic.Decorators;

public sealed class AddCharacteristicCacheCheckDecorator(
    IRequestHandler<AddCharacteristicRequest, Result> handler,
    CharacteristicsCache cache
) : IRequestHandler<AddCharacteristicRequest, Result>
{
    private readonly IRequestHandler<AddCharacteristicRequest, Result> _handler = handler;
    private readonly CharacteristicsCache _cache = cache;

    public async Task<Result> Handle(
        AddCharacteristicRequest request,
        CancellationToken ct = default
    ) =>
        _cache.Contains(request.Characteristic)
            ? new Error(
                $"Characteristic with name: ${request.Characteristic.Name} exists in cache."
            )
            : await _handler.Handle(request, ct);
}
