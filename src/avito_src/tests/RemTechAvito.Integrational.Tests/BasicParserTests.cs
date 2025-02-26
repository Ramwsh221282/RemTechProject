using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.DependencyInjection;
using Serilog;

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
        collection.RegisterServices();
        collection.AddLogging();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }
}
