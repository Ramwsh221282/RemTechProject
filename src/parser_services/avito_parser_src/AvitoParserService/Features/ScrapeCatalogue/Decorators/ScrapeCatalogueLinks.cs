using System.Text;
using System.Text.RegularExpressions;
using PuppeteerSharp;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageCreation;
using ILogger = Serilog.ILogger;
using IPageBehavior = SharedParsersLibrary.Puppeteer.Features.PageBehavior.IPageBehavior;

namespace AvitoParserService.Features.ScrapeCatalogue.Decorators;

public sealed class ScrapeCatalogueLinks : IScrapeAdvertisementsHandler
{
    private static readonly Regex _regex = new Regex(@"(\d+)\?context", RegexOptions.Compiled);
    private const string containerSelector = "items-items-pZX46";
    private const string itemSelector = "iva-item-root-Se7z4";
    private const string sliderClass = "iva-item-sliderLink-Fvfau";
    private const string photoSliderList = "photo-slider-list-jdIUv";
    private const string photoSliderItem = "photo-slider-list-item-iMp1E";
    private const string href = "href";
    private const string avitoDomain = "https://www.avito.ru";
    private readonly ScrapeCatalogueContext _context;
    private readonly ILogger _logger;
    private readonly IScrapeAdvertisementsHandler _handler;

    public ScrapeCatalogueLinks(
        IScrapeAdvertisementsHandler handler,
        ScrapeCatalogueContext context,
        ILogger logger
    )
    {
        _logger = logger;
        _context = context;
        _handler = handler;
    }

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        _logger.Information(
            "{Context} Scrape links operation started.",
            nameof(ScrapeAdvertisementCommand)
        );
        IBrowser browser = _context.Browser.Value;
        IPageBehavior scrollBottom = new ScrollBottomBehavior();
        IPageBehavior scrollTop = new ScrollTopBehavior();
        IPageBehavior<IElementHandle?> getContainer = new GetElementWithSelectorFormat(
            containerSelector
        );
        IElementBehavior<IElementHandle[]> getItems = new GetChildrenWithFormatterBehavior(
            itemSelector
        );
        IElementBehavior<IElementHandle?> getSlider = new GetChildWithFormatterBehavior(
            sliderClass
        );
        IElementBehavior<string?> getAttribute = new GetElementAttributeBehavior(href);
        foreach (var pageUrl in _context.EnumeratePages())
        {
            IPageCreationStrategy strategy = new DomLoadPageCreationStrategy(browser);
            PageFactory factory = new PageFactory(strategy);
            IPage page = await factory.Create(pageUrl);
            PageBehaviorExecutor pageExecutor = new PageBehaviorExecutor(page);
            await pageExecutor.Invoke(scrollBottom);
            await pageExecutor.Invoke(scrollTop);
            await using IElementHandle? container = await pageExecutor.Invoke(getContainer);
            if (container == null)
            {
                await page.DisposeAsync();
                break;
            }

            ElementBehaviorExecutor elementExecutor = new ElementBehaviorExecutor(container);
            IElementHandle[] items = await elementExecutor.Invoke(getItems);
            foreach (var item in items)
            {
                await using (item)
                {
                    await item.FocusAsync();
                    await item.HoverAsync();
                    elementExecutor.SwapElement(item);
                    await using IElementHandle? slider = await elementExecutor.Invoke(getSlider);
                    if (slider == null)
                        continue;
                    elementExecutor.SwapElement(slider);
                    string? attribute = await elementExecutor.Invoke(getAttribute);
                    if (string.IsNullOrWhiteSpace(attribute))
                        continue;
                    elementExecutor.SwapElement(item);
                    await using IElementHandle? photoSlider = await elementExecutor.Invoke(
                        new GetChildWithFormatterBehavior(photoSliderList)
                    );
                    string advertisementUrl = BuildAdvertisementUrl(attribute);
                    long advertisementId = long.Parse(
                        _regex.Match(advertisementUrl).Groups[1].Value
                    );
                    ScrapedAdvertisement advertisement = new ScrapedAdvertisement();
                    advertisement.SourceUrl = advertisementUrl;
                    advertisement.AdvertisementId = advertisementId;
                    if (photoSlider == null)
                    {
                        _context.AddAdvertisement(advertisement);
                        continue;
                    }

                    elementExecutor.SwapElement(photoSlider);
                    await photoSlider.FocusAsync();
                    await photoSlider.HoverAsync();
                    IElementHandle[] photos = await elementExecutor.Invoke(
                        new GetChildrenWithFormatterBehavior(photoSliderItem)
                    );
                    List<string> photoArray = [];
                    for (int i = 0; i < photos.Length; i++)
                    {
                        elementExecutor.SwapElement(photos[i]);
                        await using IElementHandle? imgElement = await elementExecutor.Invoke(
                            new GetChildBehavior("img")
                        );
                        if (imgElement == null)
                            break;

                        elementExecutor.SwapElement(imgElement);
                        string? photoAttribute = await elementExecutor.Invoke(
                            new GetElementAttributeBehavior("srcset")
                        );
                        if (string.IsNullOrWhiteSpace(photoAttribute))
                            break;

                        string[] sizes = photoAttribute.Split(
                            ',',
                            StringSplitOptions.RemoveEmptyEntries
                        );
                        string url = sizes[^1].Split(' ')[0];
                        photoArray.Add(url);
                    }

                    advertisement.PhotoUrls = photoArray.ToArray();
                    _context.AddAdvertisement(advertisement);
                }
            }

            await page.DisposeAsync();
        }

        _logger.Information(
            "{Context} Scrape links operation finished.",
            nameof(ScrapeAdvertisementCommand)
        );
        _logger.Information(
            "{Context} links to scrape: {Count}",
            nameof(ScrapeAdvertisementCommand),
            _context.AdvertisementsAmount
        );
        await _handler.Handle(command);
    }

    private static string BuildAdvertisementUrl(string hrefValue)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(avitoDomain);
        stringBuilder.Append(hrefValue);
        return stringBuilder.ToString();
    }
}
