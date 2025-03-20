using SharedParsersLibrary.ParserBehaviorFacade;

namespace AvitoParserService;

public class Worker(ParserManagementFacade facade) : BackgroundService
{
    private readonly ParserManagementFacade _facade = facade;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _facade.InvokeBasicBehavior(stoppingToken);
        }
    }
}
