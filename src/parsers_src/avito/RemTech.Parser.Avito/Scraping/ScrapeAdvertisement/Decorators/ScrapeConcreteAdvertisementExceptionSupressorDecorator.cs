using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.OptionPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement.Decorators;

public sealed class ScrapeConcreteAdvertisementExceptionSupressorDecorator(
    ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler,
    Serilog.ILogger logger
) : ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>>
{
    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler = handler;

    private readonly Serilog.ILogger _logger = logger;

    public async Task<Option<ScrapedAdvertisement>> Handle(
        ScrapeConcreteAdvertisementCommand command,
        CancellationToken ct = default
    )
    {
        try
        {
            Option<ScrapedAdvertisement> result = await _handler.Handle(command, ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Context}. Failed. Exception: {Ex}. Trace: {Trace} Source: {Source}",
                nameof(ScrapeConcreteAdvertisementCommand),
                ex.Message,
                ex.StackTrace,
                ex.Source
            );
            return Option<ScrapedAdvertisement>.None();
        }
    }
}
