using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;

public sealed class GetAdvertisementsQueryLoggingDecorator(
    IQueryHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > handler,
    Serilog.ILogger logger
)
    : IQueryHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    >
{
    private readonly IQueryHandler<
        GetAdvertisementsQuery,
        Result<(TransportAdvertisement[] advertisements, long count)>
    > _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Option<Result<(TransportAdvertisement[] advertisements, long count)>>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        _logger.Information("{Context} processing request.", nameof(GetAdvertisementsQuery));
        var result = await _handler.Handle(query, ct);
        if (!result.HasValue)
        {
            _logger.Error(
                "{Context} result has no value. Finished.",
                nameof(GetAdvertisementsQuery)
            );
            return Option<Result<(TransportAdvertisement[], long)>>.Some(
                new Error("Result has no value. Internal server error.")
            );
        }
        var data = result.Value;
        if (data.IsFailure)
            _logger.LogError(data.Error, nameof(GetAdvertisementsQuery));
        _logger.Error("{Context} Finished.", nameof(GetAdvertisementsQuery));
        return result;
    }
}
