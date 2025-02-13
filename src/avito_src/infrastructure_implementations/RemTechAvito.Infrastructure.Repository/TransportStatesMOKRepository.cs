using RemTechAvito.Core.FiltersManagement.TransportStates;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository;

public sealed class TransportStatesMOKRepository : ITransportStatesRepository
{
    private readonly List<TransportStatesCollection> _data = [];
    private readonly ILogger _logger;

    public TransportStatesMOKRepository(ILogger logger) => _logger = logger;

    public async Task<Result> Add(
        TransportStatesCollection collection,
        CancellationToken ct = default
    )
    {
        _logger.Information(
            "{Repository} method {Method} has began.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        _data.Add(collection);

        _logger.Information(
            "{Repository} method {Method} success.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        return await Task.FromResult(Result.Success());
    }
}
