using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Server.Abstractions.Communication;
using Serilog;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class ContractsResolvingCenter
{
    private readonly IServiceScopeFactory _factory;

    private readonly ILogger _logger;

    // first type is IContract. Second is IContractHandler<IContract>.
    private readonly Dictionary<Type, Type> _contractHandlers;

    public ContractsResolvingCenter(
        IServiceScopeFactory factory,
        Dictionary<Type, Type> types,
        ILogger logger
    )
    {
        _factory = factory;
        _contractHandlers = types;
        _logger = logger;
    }

    public async Task<string> ResolveContract(IContract contract)
    {
        Type requestType = contract.GetType();

        _logger.Information("Received request of type: {RequestType}", requestType.Name);

        if (!_contractHandlers.ContainsKey(requestType))
        {
            _logger.Error("Request of type: {RequestType} is not registered", requestType.Name);
            return "{}";
        }

        Type handlerInterface = typeof(IContractHandler<>).MakeGenericType(requestType);

        using IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        dynamic handler = provider.GetRequiredService(handlerInterface);
        string result = await handler.Handle((dynamic)contract);
        _logger.Information(
            "Request of type: {RequestType} is handled. Result: {Result}",
            requestType.Name,
            result
        );
        return result;
    }
}
