using Rabbit.RPC.Server.Abstractions.Communication;

namespace WebDriver.Worker.Service;

public class Worker : BackgroundService
{
    private readonly IListeningPoint _point;
    private readonly ILogger<Worker> _logger;

    public Worker(IListeningPoint point, ILogger<Worker> logger)
    {
        _point = point;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker Service has been started");
        await _point.InitializeListener();
        await Task.Delay(Timeout.Infinite, stoppingToken);
        await _point.DisposeAsync();
    }
}
