using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;

public sealed class GetAdvertisementsQueryLoggingDecorator(
    IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > handler,
    Serilog.ILogger logger
)
    : IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    >
{
    private readonly IRequestHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Result<(TransportAdvertisement[] advertisements, long count)>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        _logger.Information("{Context} processing request.", nameof(GetAdvertisementsQuery));
        var result = await _handler.Handle(query, ct);
        return result
            .ToWhen()
            .IfFailure(() => _logger.LogError(result.Error, nameof(GetAdvertisementsQuery)))
            .IfSuccess(() => _logger.Error("{Context} Finished.", nameof(GetAdvertisementsQuery)))
            .BackToResult();
    }
}
