using System.Reflection;
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

    public void RegisterContractsFromEntryAssembly()
    {
        Assembly? assembly = Assembly.GetEntryAssembly();
        if (assembly == null)
            return;

        Type[] contractTypes = assembly
            .GetTypes()
            .Where(t => typeof(IContract).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        foreach (var contractType in contractTypes)
        {
            Type? handlerType = assembly
                .GetTypes()
                .FirstOrDefault(t =>
                    typeof(IContractHandler<>).MakeGenericType(contractType).IsAssignableFrom(t)
                );

            if (handlerType == null)
                continue;

            string operationName = contractType.Name;
            _contractTypes.TryAdd(operationName, contractType);
            _contractsAndHandlerTypes.TryAdd(contractType, handlerType);
        }
    }

    public void RegisterConnectionFactory(ICustomConnectionFactory factory) => _factory = factory;

    public void RegisterServer(IServiceCollection services)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger();
        services.AddSingleton(logger);

        if (_contractTypes.Count == 0)
            throw new InvalidOperationException("No contracts have been configured.");

        services.AddSingleton<ICustomConnectionFactory>(p =>
        {
            IRabbitMqListenerOptions options = p.GetRequiredService<IRabbitMqListenerOptions>();
            return new SimpleConnectionFactory(
                options.HostName,
                options.UserName,
                options.Password
            );
        });

        IncomingRequestMapper mapper = new IncomingRequestMapper(_contractTypes, logger);
        services.AddSingleton(mapper);

        foreach (var (requestType, handlerType) in _contractsAndHandlerTypes)
        {
            Type interfaceType = typeof(IContractHandler<>).MakeGenericType(requestType);
            services = services.AddScoped(interfaceType, handlerType);
        }

        services.AddSingleton<ContractsResolvingCenter>(p =>
        {
            IServiceScopeFactory factory = p.GetRequiredService<IServiceScopeFactory>();
            ContractsResolvingCenter center = new ContractsResolvingCenter(
                factory,
                _contractsAndHandlerTypes,
                logger
            );
            return center;
        });

        services.AddSingleton<IServerProcess>(p =>
        {
            ContractsResolvingCenter resolving = p.GetRequiredService<ContractsResolvingCenter>();
            IncomingRequestMapper mapping = p.GetRequiredService<IncomingRequestMapper>();
            IServerProcess process = new SimpleServerProcess(mapping, resolving);
            return process;
        });

        services.AddSingleton<IListeningPoint>(p =>
        {
            ICustomConnectionFactory factory = p.GetRequiredService<ICustomConnectionFactory>();
            IServerProcess process = p.GetRequiredService<IServerProcess>();
            IRabbitMqListenerOptions options = p.GetRequiredService<IRabbitMqListenerOptions>();
            SimpleListeningPoint point = new SimpleListeningPoint(
                factory,
                process,
                options.QueueName,
                logger
            );
            return point;
        });
    }

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
            IServerProcess process = new SimpleServerProcess(mapping, resolving);
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
