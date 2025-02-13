using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebDriver.Worker.Service;

namespace RemTechAvito.Integrational.Tests;

public abstract class BasicParserTests
{
    protected const string host = "localhost";
    protected const string user = "rmuser";
    protected const string password = "rmpassword";
    protected const string queue = "web-driver-service";

    protected readonly ILogger _logger;

    protected readonly IServiceProvider _serviceProvider;

    protected BasicParserTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, host, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }
}
