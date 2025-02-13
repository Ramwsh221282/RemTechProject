using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Infrastructure.Repository;

public sealed class CustomerTypesRepository(ILogger logger) : ICustomerTypesRepository
{
    private readonly List<CustomerTypesCollection> _data = [];

    public async Task<Result> Add(
        CustomerTypesCollection collection,
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
