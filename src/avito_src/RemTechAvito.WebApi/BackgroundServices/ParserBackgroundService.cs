using Rabbit.RPC.Client.Abstractions;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.FiltersManagement.TransportTypes.Commands.ParseTransportTypes;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.ParseTransportAdvertisementsCatalogue;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Infrastructure.Contracts.Repository;
using WebDriver.Worker.Service.Contracts.BaseContracts;
using ILogger = Serilog.ILogger;

namespace RemTechAvito.WebApi.BackgroundServices;

public sealed class ParserBackgroundService(ILogger logger, IServiceScopeFactory factory)
    : BackgroundService
{
    public bool IsWorking { get; private set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        IsWorking = true;
        await ProcessParsing(stoppingToken);
        await RefreshTransportTypesCollection(stoppingToken);
        logger.Information("{Service} sleeping for 6 hours.", nameof(ParserBackgroundService));
        await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
        IsWorking = false;
    }

    private async Task ProcessParsing(CancellationToken ct)
    {
        var urls = await GetUrls(ct);
        IsWorking = true;
        foreach (var url in urls)
            try
            {
                await PerformParsing(url, ct);
            }
            catch (Exception ex)
            {
                logger.Fatal(
                    "{Service}. Cannot parse url {url}. Exception: {Ex}",
                    nameof(ParserBackgroundService),
                    url,
                    ex.Message
                );
            }
    }

    private async Task<IEnumerable<string>> GetUrls(CancellationToken ct)
    {
        using var scope = factory.CreateScope();
        var provider = scope.ServiceProvider;
        var repository = provider.GetRequiredService<IParserProfileReadRepository>();
        var profiles = await repository.GetActiveOnly(ct);
        return profiles.SelectMany(p => p.Links).Select(l => l.Link).Distinct().ToArray();
    }

    private async Task PerformParsing(string url, CancellationToken ct)
    {
        using var scope = factory.CreateScope();
        var provider = scope.ServiceProvider;
        var command = new ParseTransportAdvertisementCatalogueCommand(url);
        var handler = provider.GetRequiredService<
            IAvitoCommandHandler<ParseTransportAdvertisementCatalogueCommand>
        >();
        await handler.Handle(command, ct);
    }

    private async Task RefreshTransportTypesCollection(CancellationToken ct)
    {
        using var scope = factory.CreateScope();
        try
        {
            var provider = scope.ServiceProvider;
            var command = new ParseTransportTypesCommand();
            var handler = provider.GetRequiredService<
                IAvitoCommandHandler<ParseTransportTypesCommand, TransportTypeResponse>
            >();
            await handler.Handle(command, ct);
            logger.Information(
                "{Service} refreshed transport types",
                nameof(ParserBackgroundService)
            );
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Service}. Cannot refresh transport types. Exception: {Ex}",
                nameof(ParserBackgroundService),
                ex.Message
            );
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Service} started.", nameof(ParserBackgroundService));
        return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        logger.Information("{Service} stopped.", nameof(ParserBackgroundService));
        var publisher = new SingleCommunicationPublisher(queue, host, user, password);
        publisher.Send(new StopWebDriverContract(), cancellationToken);
        return base.StopAsync(cancellationToken);
    }

    // TEMPORARY. WILL BE REMOVED.
    private const string host = "localhost";
    private const string user = "rmuser";
    private const string password = "rmpassword";
    private const string queue = "web-driver-service";
}
