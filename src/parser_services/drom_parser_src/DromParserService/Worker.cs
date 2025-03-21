using SharedParsersLibrary.ParserBehaviorFacade;

namespace DromParserService;

public class Worker(ParserManagementFacade facade) : BackgroundService
{
    private readonly ParserManagementFacade _facade = facade;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _facade.ParserName = "DROM";
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _facade.InvokeBasicBehavior(stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
