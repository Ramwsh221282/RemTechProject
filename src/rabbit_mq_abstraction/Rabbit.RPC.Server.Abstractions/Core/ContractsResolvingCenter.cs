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
            ReadOnlyMemory<char> requestTypeName = requestType.Name.AsMemory();
            _logger.Information("Server Received request: {RequestType}", requestTypeName);

            if (!_contractHandlers.ContainsKey(requestType))
            {
                _logger.Error("Server Request {Type} is not allowed.", requestTypeName);
                return ContractActionResult.Fail($"Request {requestTypeName} is not allowed.");
            }

            Type handlerInterface = typeof(IContractHandler<>).MakeGenericType(requestType);
            using IServiceScope scope = _factory.CreateScope();
            IServiceProvider provider = scope.ServiceProvider;

            object handler = provider.GetRequiredService(handlerInterface);
            MethodInfo? handlerMethod = handlerInterface.GetMethod("Handle");
            if (handlerMethod == null)
            {
                _logger.Error(
                    "Server Cannot resolve request: {Type} because no handlers registered.",
                    requestTypeName
                );
                return ContractActionResult.Fail(
                    $"Server Cannot resolve request: {requestTypeName} because no handlers registered."
                );
            }

            Task<ContractActionResult> task =
                (Task<ContractActionResult>)
                    handlerMethod.Invoke(handler, new object[] { contract })!;
            ContractActionResult response = await task;

            _logger.Information(
                "Server Request of type: {RequestType} is handled",
                requestTypeName
            );
            return response;
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            _logger.Error("Service exception: {Exception}", message.AsMemory());
            return ContractActionResult.Fail(message);
        }
    }
}
