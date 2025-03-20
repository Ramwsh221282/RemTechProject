using AvitoParserService.Common.Cqrs;
using AvitoParserService.Features.ScrapeAdvertisement;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.PageCreation;
using SharedParsersLibrary.Sinking;

namespace AvitoParserService.Features.ScrapeCatalogue;

public sealed class ScrapeCatalogueHandler : IScrapeAdvertisementsHandler
{
    private readonly SinkerSenderFactory _factory;
    private readonly ScrapeCatalogueContext _context;

    private readonly ICommandHandler<
        ScrapeConcreteAdvertisementCommand,
        Option<ScrapedAdvertisement>
    > _handler;

    public ScrapeCatalogueHandler(
        SinkerSenderFactory factory,
        ScrapeCatalogueContext context,
        ICommandHandler<ScrapeConcreteAdvertisementCommand, Option<ScrapedAdvertisement>> handler
    )
    {
        _factory = factory;
        _context = context;
        _handler = handler;
    }

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        IBrowser browser = _context.Browser.Value;
        foreach (ScrapedAdvertisement advertisement in _context.EnumerateAdvertisements())
        {
            var pageFactory = new PageFactory(new DomLoadPageCreationStrategy(browser));
            var page = await pageFactory.Create(advertisement.SourceUrl);
            var subCommand = new ScrapeConcreteAdvertisementCommand(page, advertisement);

            try
            {
                var result = await _handler.Handle(subCommand);
                await Task.Delay(TimeSpan.FromSeconds(5));
                if (result.HasValue)
                {
                    using var sinker = _factory.CreateSinker();
                    sinker.Sink(result.Value, "AVITO");
                }
            }
            catch
            {
                await page.DisposeAsync();
            }
        }

        await browser.DisposeAsync();
    }
}
