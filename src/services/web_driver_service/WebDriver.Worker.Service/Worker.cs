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
        try { }
        catch (Exception ex)
        {
            Console.WriteLine($"Logger error: {ex.Message}");
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _point.InitializeListener();
            await base.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logger error. {ex.Message}");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _point.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Logger error. {ex.Message}");
        }
    }
}
