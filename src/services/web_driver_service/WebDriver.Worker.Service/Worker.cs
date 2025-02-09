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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!_point.IsInitialized)
        {
            await _point.InitializeListener();
        }
        await Task.Delay(Timeout.Infinite, stoppingToken);
        await _point.DisposeAsync();
    }
}
