using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Server.Abstractions.Communication;
using Serilog;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class ServerRegistrationContext
{
    private readonly Dictionary<string, Type> _contractTypes = [];
    private readonly Dictionary<Type, Type> _contractsAndHandlerTypes = [];
    private ICustomConnectionFactory? _factory;

    public void RegisterContract<TContract, TContractHandler>()
        where TContract : class, IContract
        where TContractHandler : class, IContractHandler<TContract>
    {
        string operationName = typeof(TContract).Name;
        if (_contractTypes.ContainsKey(operationName))
            return;
        _contractTypes.Add(operationName, typeof(TContract));
        _contractsAndHandlerTypes.Add(typeof(TContract), typeof(TContractHandler));
    }

    public void RegisterConnectionFactory(ICustomConnectionFactory factory) => _factory = factory;

    public IServiceCollection RegisterServer(IServiceCollection services, string queueName)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger();
        services = services.AddSingleton(logger);

        if (_factory == null)
            throw new InvalidOperationException("No factory has been configured.");

        if (_contractTypes.Count == 0)
            throw new InvalidOperationException("No contracts have been configured.");

        services = services.AddSingleton(_factory);

        IncomingRequestMapper mapper = new IncomingRequestMapper(_contractTypes, logger);
        services = services.AddSingleton(mapper);

        foreach (var (requestType, handlerType) in _contractsAndHandlerTypes)
        {
            Type interfaceType = typeof(IContractHandler<>).MakeGenericType(requestType);
            services = services.AddScoped(interfaceType, handlerType);
        }

        services = services.AddSingleton<ContractsResolvingCenter>(p =>
        {
            IServiceScopeFactory factory = p.GetRequiredService<IServiceScopeFactory>();
            ContractsResolvingCenter center = new ContractsResolvingCenter(
                factory,
                _contractsAndHandlerTypes,
                logger
            );
            return center;
        });

        services = services.AddSingleton<IServerProcess>(p =>
        {
            ContractsResolvingCenter resolving = p.GetRequiredService<ContractsResolvingCenter>();
            IncomingRequestMapper mapping = p.GetRequiredService<IncomingRequestMapper>();
            IServerProcess process = new SimpleServerProcess(mapping, resolving, logger);
            return process;
        });

        services.AddSingleton<IListeningPoint>(p =>
        {
            ICustomConnectionFactory factory = p.GetRequiredService<ICustomConnectionFactory>();
            IServerProcess process = p.GetRequiredService<IServerProcess>();
            SimpleListeningPoint point = new SimpleListeningPoint(
                factory,
                process,
                queueName,
                logger
            );
            return point;
        });

        return services;
    }
}
