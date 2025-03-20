using Microsoft.Extensions.DependencyInjection;
using RemTechCommon.Utils.OptionPattern;
using Serilog;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.ParserMessaging;

namespace SharedParsersLibrary.ParserBehaviorFacade;

public sealed class ParserManagementFacade(
    ILogger logger,
    IServiceProvider provider,
    ParserMessageSender sender
)
{
    private readonly ILogger _logger = logger;
    private readonly IServiceProvider _provider = provider;
    private readonly ParserMessageSender _sender = sender;
    public string ParserName { get; set; } = string.Empty;

    public async Task InvokeBasicBehavior(CancellationToken ct = default)
    {
        _logger.Information("{Context} Requesting parser configuration...", ParserName);
        Option<Parser> parserRequest = await _sender.RequestParser(ParserName);
        if (!parserRequest.HasValue)
        {
            _logger.Warning("{Context} Configuration for Parser Service does not exist.");
            await SleepForMinute();
        }

        Parser parser = parserRequest.Value;
        if (parser.State == "Отключен")
        {
            _logger.Warning("{Context} Parsing was not started. State is disabled.", ParserName);
            await SleepForMinute();
        }

        if (parser.Links.Length == 0)
        {
            _logger.Warning("{Context} Parser was not started. Links count 0.", ParserName);
            await SleepForMinute();
        }

        parser = parser.SetWorkingState().UpdateSchedule();
        await _sender.UpdateParser(parser);

        _logger.Information(
            "{Context} detected {Count} links. Start scraping...",
            ParserName,
            parser.Links.Length
        );

        await InvokeScraping(parser);
        _logger.Information("{Context} finished parsing.", ParserName);
        parser = parser.SetSleepingState();
        await _sender.UpdateParser(parser);
        _logger.Information(
            "{Context} sleeping for {Count} hours.",
            ParserName,
            parser.RepeatEveryHours
        );
        await Task.Delay(TimeSpan.FromHours(parser.RepeatEveryHours));
    }

    private async Task SleepForMinute()
    {
        _logger.Information("{Context} sleeping for 1 minute...", ParserName);
        await Task.Delay(TimeSpan.FromMinutes(1));
    }

    private async Task InvokeScraping(Parser parser)
    {
        foreach (var link in parser.Links)
        {
            try
            {
                var command = new ScrapeAdvertisementCommand(link);
                var handler = _provider.GetRequiredService<IScrapeAdvertisementsHandler>();
                await handler.Handle(command);
                _logger.Information("{Context} Scraped {Link}", ParserName, link);
            }
            catch (Exception ex)
            {
                _logger.Fatal(
                    "{Context} Scraping {Link} Resulted in Exception: {Message}",
                    ParserName,
                    link,
                    ex.Message
                );
            }
        }
    }
}
