using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository;

public sealed class CustomerStatesMOKRepository(ILogger logger) : ICustomerStatesRepository
{
    private readonly List<CustomerStatesCollection> _collection = [];

    public async Task<Result> Add(
        CustomerStatesCollection collection,
        CancellationToken ct = default
    )
    {
        logger.Information(
            "{Repository} method {Method} has began.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        _collection.Add(collection);

        logger.Information(
            "{Repository} method {Method} success.",
            nameof(TransportTypesMOKRepository),
            nameof(Add)
        );

        return await Task.FromResult(Result.Success());
    }
}
