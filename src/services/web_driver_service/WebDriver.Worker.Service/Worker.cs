using Rabbit.RPC.Server.Abstractions.Communication;

namespace WebDriver.Worker.Service;

public class Worker : BackgroundService
{
    private readonly IListeningPoint _point;

    public Worker(IListeningPoint point)
    {
        _point = point;
        _point.ServiceName = "WebDriver.Worker.Service";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) { }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await _point.InitializeListener();
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _point.DisposeAsync();
        await base.StopAsync(cancellationToken);
    }
}
