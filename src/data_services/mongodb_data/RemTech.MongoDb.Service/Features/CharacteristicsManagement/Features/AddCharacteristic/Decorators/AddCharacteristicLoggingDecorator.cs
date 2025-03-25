using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic.Decorators;

public sealed class AddCharacteristicLoggingDecorator(
    IRequestHandler<AddCharacteristicRequest, Result> handler,
    Serilog.ILogger logger
) : IRequestHandler<AddCharacteristicRequest, Result>
{
    private readonly IRequestHandler<AddCharacteristicRequest, Result> _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result> Handle(
        AddCharacteristicRequest request,
        CancellationToken ct = default
    )
    {
        Result result = await _handler.Handle(request, ct);
        return result
            .ToWhen()
            .IfFailure(
                () =>
                    _logger.Error(
                        "{Context}. {Error}.",
                        nameof(AddCharacteristicRequest),
                        result.Error.Description
                    )
            )
            .IfSuccess(
                () =>
                    _logger.Information(
                        "{Context}. Added characteristic: {Name}",
                        nameof(AddCharacteristicRequest),
                        request.Characteristic.Name
                    )
            )
            .BackToResult();
    }
}
