using AvitoParserService.Common.Cqrs;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using ILogger = Serilog.ILogger;

namespace AvitoParserService.Features.ScrapeAdvertisement.Decorators;

public sealed class InitializeUrl
    : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ScrapeConcreteAdvertisementContext _context;
    private readonly ILogger _logger;

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    public InitializeUrl(
        ScrapeConcreteAdvertisementContext context,
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
        ILogger logger
    )
    {
        _context = context;
        _handler = handler;
        _logger = logger;
    }

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command
    )
    {
        _logger.Information(
            "{Context} navigation on advertisement page started.",
            nameof(ScrapeConcreteAdvertisementCommand)
        );
        PageBehaviorExecutor executor = new PageBehaviorExecutor(command.Page);
        IPageBehavior scrollBottom = new ScrollBottomBehavior();
        IPageBehavior scrollTop = new ScrollTopBehavior();
        await executor.Invoke(scrollBottom);
        await executor.Invoke(scrollTop);
        _context.Initialize(command.Advertisement);
        _logger.Information(
            "{Context} navigation on advertisement page finished.",
            nameof(ScrapeConcreteAdvertisementCommand)
        );
        return await _handler.Handle(command);
    }
}
