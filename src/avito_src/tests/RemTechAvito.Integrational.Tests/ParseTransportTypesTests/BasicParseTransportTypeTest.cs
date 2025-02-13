using Microsoft.Extensions.DependencyInjection;
using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Infrastructure.Contracts.Parser;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.Infrastructure.Parser;
using RemTechAvito.Infrastructure.Repository;
using RemTechCommon.Utils.ResultPattern;
using WebDriver.Worker.Service;
using ILogger = Serilog.ILogger;

namespace RemTechAvito.Integrational.Tests.ParseTransportTypesTests;

public sealed class BasicParseTransportTypeTest
{
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";

    private readonly ILogger _logger;

    private readonly IServiceProvider _serviceProvider;

    public BasicParseTransportTypeTest()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.AddLogging();
        collection.InitializeWorkerDependencies(queue, host, user, password);
        collection.AddSingleton<Worker>();
        _serviceProvider = collection.BuildServiceProvider();
        _logger = _serviceProvider.GetRequiredService<ILogger>();
    }

    // TODO Нужно добавить скроллинг элементов типа транспорта в пайплайн парсера.
    [Fact]
    public async Task Invoke_Basic()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;

        IMessagePublisher publisher = new MultiCommunicationPublisher(queue, host, user, password);
        ITransportTypesParser parser = new TransportTypesParser(publisher);
        ITransportTypesRepository repository = new TransportTypesMOKRepository(_logger);
        ParseTransportTypesCommand command = new();
        ParseTransportTypesCommandHandler handler = new(parser, repository, _logger);

        Worker worker = _serviceProvider.GetRequiredService<Worker>();
        await worker.StartAsync(ct);

        Result execution = await handler.Handle(command, ct);
        Assert.True(execution.IsSuccess);

        await worker.StopAsync(ct);
    }
}
