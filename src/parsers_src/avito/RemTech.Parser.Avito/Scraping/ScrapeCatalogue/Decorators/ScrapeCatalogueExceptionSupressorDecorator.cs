using SharedParsersLibrary.Contracts;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue.Decorators;

public sealed class ScrapeCatalogueExceptionSupressorDecorator(
    IScrapeAdvertisementsHandler handler,
    Serilog.ILogger logger
) : IScrapeAdvertisementsHandler
{
    private readonly IScrapeAdvertisementsHandler _handler = handler;
    private readonly Serilog.ILogger _logger = logger;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        try
        {
            await _handler.Handle(command);
        }
        catch (Exception ex)
        {
            _logger.Fatal(
                "{Command} operation stopped. Exception: {Ex} Trace: {Trace} Source: {Source}",
                nameof(ScrapeAdvertisementsCommand),
                ex.Message,
                ex.StackTrace,
                ex.Source
            );
        }
    }
}
