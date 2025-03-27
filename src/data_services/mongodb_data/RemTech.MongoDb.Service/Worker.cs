using Rabbit.RPC.Server.Abstractions.Communication;
using ILogger = Serilog.ILogger;

namespace RemTech.MongoDb.Service;

public class Worker : BackgroundService
{
    private readonly IListeningPoint _listeningPoint;
    private readonly ILogger _logger;

    public Worker(IListeningPoint listeningPoint, ILogger logger)
    {
        _listeningPoint = listeningPoint;
        _listeningPoint.ServiceName = "Data Service";
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _listeningPoint.InitializeListener();
        await base.StartAsync(cancellationToken);
        _logger.Information("{Service} started", _listeningPoint.ServiceName);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _listeningPoint.DisposeAsync();
        await base.StopAsync(cancellationToken);
        _logger.Information("{Service} stopped", _listeningPoint.ServiceName);
    }
}
