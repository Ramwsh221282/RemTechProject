using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements.Decorators;

public sealed class GetAdvertisementsQueryLoggingDecorator(
    IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>> handler,
    Serilog.ILogger logger
) : IQueryHandler<GetAdvertisementsQuery, Result<TransportAdvertisement[]>>
{
    private readonly IQueryHandler<
        GetAdvertisementsQuery,
        Result<TransportAdvertisement[]>
    > _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<Option<Result<TransportAdvertisement[]>>> Handle(
        GetAdvertisementsQuery query,
        CancellationToken ct = default
    )
    {
        _logger.Information("{Context} processing request.", nameof(GetAdvertisementsQuery));
        Option<Result<TransportAdvertisement[]>> result = await _handler.Handle(query, ct);
        if (!result.HasValue)
        {
            _logger.Error(
                "{Context} result has no value. Finished.",
                nameof(GetAdvertisementsQuery)
            );
            return new Error("Result has no value. Internal server error.").AsSome<
                Result<TransportAdvertisement[]>
            >();
        }
        Result<TransportAdvertisement[]> data = result.Value;
        if (data.IsFailure)
            _logger.LogError(data.Error, nameof(GetAdvertisementsQuery));
        _logger.Error("{Context} Finished.", nameof(GetAdvertisementsQuery));
        return result;
    }
}
