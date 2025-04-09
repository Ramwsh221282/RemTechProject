using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementUrlDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    ScrapeConcreteAdvertisementContext context,
    Serilog.ILogger logger
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler = handler;

    private readonly ScrapeConcreteAdvertisementContext _context = context;

    private readonly Serilog.ILogger _logger = logger;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command,
        CancellationToken ct = default
    )
    {
        _logger.Information(
            "{Context} navigation on advertisement page started.",
            nameof(ScrapeConcreteAdvertisementCommand)
        );

        await command.Page.ScrollBottom();
        await command.Page.ScrollTop();

        _context.Initialize(command.Advertisement);
        _logger.Information(
            "{Context} navigation on advertisement page finished.",
            nameof(ScrapeConcreteAdvertisementCommand)
        );

        return await _handler.Handle(command, ct);
    }
}
