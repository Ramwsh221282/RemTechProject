using System.Text;
using System.Text.RegularExpressions;
using PuppeteerSharp;
using RemTech.Parser.Avito.Scraping.Authorization;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.BrowserCreation;
using SharedParsersLibrary.Puppeteer.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Extensions;
using SharedParsersLibrary.Puppeteer.PageBehavior;

namespace RemTech.Parser.Avito.Scraping.ScrapeCatalogue.Decorators;

public sealed partial class ScrapeCatalogueItemsDecorator(
    IScrapeAdvertisementsHandler handler,
    ScrapeCatalogueContext context,
    Serilog.ILogger logger
) : IScrapeAdvertisementsHandler
{
    private static readonly Regex _regex = MyRegex();
    private const string containerSelector = "div[data-marker='catalog-serp']";
    private const string itemSelector = "div[data-marker='item']";
    private const string sliderClass = "iva-item-sliderLink-Fvfau";
    private const string photoSliderList = "photo-slider-list-jdIUv";
    private const string photoSliderItem = "photo-slider-list-item-iMp1E";
    private const string href = "href";
    private const string avitoDomain = "https://www.avito.ru";
    private readonly ScrapeCatalogueContext _context = context;
    private readonly Serilog.ILogger _logger = logger;
    private readonly IScrapeAdvertisementsHandler _handler = handler;

    public async Task Handle(ScrapeAdvertisementsCommand command)
    {
        _logger.Information("{Action} started", nameof(ScrapeCatalogueItemsDecorator));

        IBrowser browser = await BrowserFactory.CreateStealthBrowserInstance(false);

        AvitoAuthorization authorization = new(browser);
        Result authorizationResult = await authorization.Authorize();
        if (authorizationResult.IsFailure)
        {
            _logger.Error("{Action} unable to authorize.", nameof(ScrapeCatalogueItemsDecorator));
            await _handler.Handle(command);
            return;
        }

        foreach (string url in _context.EnumeratePages())
        {
            _logger.Information(
                "{Context} navigated on page: {Page}",
                nameof(ScrapeCatalogueItemsDecorator),
                url
            );

            IPage page = await browser.CreateByDomLoadNoImages(url);

            await page.ScrollBottom();

            Option<IElementHandle> container = await page.GetElementWithoutClassFormatter(
                containerSelector
            );
            if (container.HasValue == false)
            {
                _logger.Warning(
                    "{Context} page {Page} has no items. Probably last page.",
                    nameof(ScrapeCatalogueItemsDecorator),
                    url
                );
                page.Dispose();
                break;
            }

            Option<IElementHandle[]> catalogueItems =
                await container.Value.GetChildrenWithoutClassFormatter(itemSelector);
            if (catalogueItems.HasValue == false)
            {
                _logger.Warning(
                    "{Context} page {Page} has no items. Probably last page.",
                    nameof(ScrapeCatalogueItemsDecorator),
                    url
                );
                page.Dispose();
                break;
            }

            await ProcessItems(catalogueItems.Value);
            _logger.Information(
                "{Context} page {Page} items scraped.",
                nameof(ScrapeCatalogueItemsDecorator),
                url
            );

            page.Dispose();
        }

        browser.Dispose();

        _logger.Information(
            "{Context} catalogue items collecting finished.",
            nameof(ScrapeCatalogueItemsDecorator)
        );

        _logger.Information("{Action} finished.", nameof(ScrapeCatalogueItemsDecorator));
        await _handler.Handle(command);
    }

    private async Task ProcessItems(IElementHandle[] items)
    {
        foreach (IElementHandle catalogueItem in items)
        {
            // await catalogueItem.FocusAsync();
            // await catalogueItem.HoverAsync();

            Option<IElementHandle> slider = await catalogueItem.GetChildWithClassFormatter(
                sliderClass
            );
            if (slider.HasValue == false)
                continue;

            Option<string> attribute = await slider.Value.GetElementAttributeValue(href);
            if (attribute.HasValue == false)
                continue;

            Option<IElementHandle> photoSlider = await catalogueItem.GetChildWithClassFormatter(
                photoSliderList
            );
            if (photoSlider.HasValue == false)
                continue;

            string advertisementUrl = BuildAdvertisementUrl(attribute.Value);
            long advertisementId = long.Parse(_regex.Match(advertisementUrl).Groups[1].Value);
            string[] photos = await ScrapePhotos(photoSlider.Value);

            ScrapedAdvertisement advertisement = ScrapedAdvertisement.Default();
            advertisement = advertisement with
            {
                SourceUrl = advertisementUrl,
                Id = advertisementId,
                PhotoUrls = photos,
            };

            _context.AddAdvertisement(advertisement);
        }
    }

    private static async Task<string[]> ScrapePhotos(IElementHandle photoSlider)
    {
        await photoSlider.FocusAsync();
        await photoSlider.HoverAsync();

        Option<IElementHandle[]> photos = await photoSlider.GetChildrenWithClassFormatter(
            photoSliderItem
        );
        if (photos.HasValue == false)
            return [];

        List<string> photoUrls = [];

        foreach (IElementHandle photo in photos.Value)
        {
            Option<IElementHandle> image = await photo.GetChildWithoutClassFormatter("img");
            if (image.HasValue == false)
                break;

            Option<string> photoAttribute = await image.Value.GetElementAttributeValue("srcset");
            if (photoAttribute.HasValue == false)
                break;

            string[] sizes = photoAttribute.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            string url = sizes[^1].Split(' ')[0];
            photoUrls.Add(url);
        }

        return photoUrls.ToArray();
    }

    [GeneratedRegex(@"(\d+)\?context", RegexOptions.Compiled)]
    private static partial Regex MyRegex();

    private static string BuildAdvertisementUrl(string hrefValue)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(avitoDomain);
        stringBuilder.Append(hrefValue);
        return stringBuilder.ToString();
    }
}
