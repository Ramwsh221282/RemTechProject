using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic.Decorators;

public sealed class AddCharacteristicRepositoryCheckDecorator(
    IRequestHandler<AddCharacteristicRequest, Result> handler,
    CharacteristicsRepository repository
) : IRequestHandler<AddCharacteristicRequest, Result>
{
    private readonly IRequestHandler<AddCharacteristicRequest, Result> _handler = handler;
    private readonly CharacteristicsRepository _repository = repository;

    public async Task<Result> Handle(
        AddCharacteristicRequest request,
        CancellationToken ct = default
    ) =>
        await _repository.Contains(request.Characteristic)
            ? new Error(
                $"Characteristic with name {request.Characteristic.Name} already exists in database"
            )
            : await _handler.Handle(request, ct);
}
