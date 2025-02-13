using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository;

public sealed class TransportTypesMOKRepository(ILogger logger) : ITransportTypesRepository
{
    private readonly List<TransportTypesCollection> _data = [];

    public async Task<Result> Add(
        TransportTypesCollection collection,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Repository} method {Method} has began.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        _data.Add(collection);

        logger.Information(
            "{Repository} method {Method} success.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        return await Task.FromResult(Result.Success());
    }
}
