using PuppeteerSharp;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.ConcreteAdvertisementScraping;

public sealed record ScrapeConcreteAdvertisementCommand(
    IPage Page,
    ScrapedAdvertisement Advertisement
);
