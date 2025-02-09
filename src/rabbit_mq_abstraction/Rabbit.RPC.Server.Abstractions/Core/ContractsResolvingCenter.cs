using System.Reflection;
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

    public async Task<ContractActionResult> ResolveContract(IContract contract)
    {
        try
        {
            Type requestType = contract.GetType();
            _logger.Information("Received request: {RequestType}", requestType.Name);

            if (!_contractHandlers.ContainsKey(requestType))
            {
                _logger.Error("Request {Type} is not allowed.", requestType.Name);
                return new($"Request {requestType.Name} is not allowed.");
            }

            Type handlerInterface = typeof(IContractHandler<>).MakeGenericType(requestType);
            using IServiceScope scope = _factory.CreateScope();
            IServiceProvider provider = scope.ServiceProvider;

            object handler = provider.GetRequiredService(handlerInterface);
            MethodInfo? handlerMethod = handlerInterface.GetMethod("Handle");
            if (handlerMethod == null)
            {
                _logger.Error(
                    "Cannot resolve request: {Type} because no handlers registered.",
                    requestType.Name
                );
                return new ContractActionResult(
                    $"Cannot resolve request: {requestType.Name} because no handlers registered."
                );
            }
            Task<ContractActionResult> task =
                (Task<ContractActionResult>)
                    handlerMethod.Invoke(handler, new object[] { contract })!;
            ContractActionResult response = await task;
            _logger.Information("Request of type: {RequestType} is handled", requestType.Name);
            return response;
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            _logger.Error("Service exception: {Exception}", message);
            return new ContractActionResult(message);
        }
    }
}
