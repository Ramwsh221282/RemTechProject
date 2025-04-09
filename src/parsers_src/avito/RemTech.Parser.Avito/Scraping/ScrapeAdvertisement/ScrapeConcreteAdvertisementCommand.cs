using PuppeteerSharp;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Avito.Scraping.ScrapeAdvertisement;

public sealed record ScrapeConcreteAdvertisementCommand(
    IPage Page,
    ScrapedAdvertisement Advertisement
) : ICommand;
