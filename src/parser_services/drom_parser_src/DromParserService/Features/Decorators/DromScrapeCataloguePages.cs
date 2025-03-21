using System.Text;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;
using SharedParsersLibrary.Puppeteer.Features.ElementBehavior;
using SharedParsersLibrary.Puppeteer.Features.PageBehavior;
using ILogger = Serilog.ILogger;

namespace DromParserService.Features.Decorators;

public sealed class DromScrapeCataloguePages(
    IScrapeAdvertisementsHandler handler,
    DromCatalogueScrapingContext context,
    ILogger logger
) : IScrapeAdvertisementsHandler
{
    private const string itemsContainerClassName = "div[data-bulletin-list='true']";
    private const string itemClassName = "css-1f68fiz ea1vuk60";
    private const string imagePreviewClassName = "emt6rd1";
    private readonly IScrapeAdvertisementsHandler _handler = handler;
    private readonly DromCatalogueScrapingContext _context = context;
    private readonly ILogger _logger = logger;

    public async Task Handle(ScrapeAdvertisementCommand command)
    {
        IBrowser browser = _context.Browser.Value;
        int currentPage = 1;
        while (true)
        {
            string pageUrl = CreatePageUrl(command.Url, ref currentPage);
            await using IPage page = await browser.CreateByDomLoadStrategy(pageUrl);
            _logger.Information("{Context} page navigated.", nameof(ScrapeAdvertisementCommand));
            await page.ScrollBottom();
            await page.ScrollTop();

            Option<IElementHandle[]> itemsOption = await GetItemsArray(page);
            if (!itemsOption.HasValue)
            {
                _logger.Information("{Context} end reached", nameof(ScrapeAdvertisementCommand));
                break;
            }

            Dictionary<IElementHandle, ScrapedAdvertisement> advertisements = AsAdvertisements(
                itemsOption.Value
            );

            await InvokeImageAppearing(advertisements);

            itemsOption = await GetItemsArray(page);
            if (!itemsOption.HasValue)
            {
                _logger.Information(
                    "{Context} can't get items after image preview invoking",
                    nameof(ScrapeAdvertisementCommand)
                );
                break;
            }

            advertisements = AsAdvertisements(itemsOption.Value);
            await InitializeAdvertisementsImages(advertisements);
            await InitializeInfoFromTitle(advertisements);
            InitializeIds(advertisements);
            await AppendInContext(advertisements);
            currentPage++;
        }
        await _handler.Handle(command);
    }

    private static async Task<Option<IElementHandle[]>> GetItemsArray(IPage page)
    {
        Option<IElementHandle> container = await page.GetElementWithoutClassFormatter(
            itemsContainerClassName
        );
        if (!container.HasValue)
            return Option<IElementHandle[]>.None();
        await using IElementHandle containerElement = container.Value;
        return await containerElement.GetChildrenWithClassFormatter(itemClassName);
    }

    private static Dictionary<IElementHandle, ScrapedAdvertisement> AsAdvertisements(
        IElementHandle[] elements
    )
    {
        Dictionary<IElementHandle, ScrapedAdvertisement> data = [];
        foreach (var element in elements)
            data.Add(element, new ScrapedAdvertisement());
        return data;
    }

    private static async Task InvokeImageAppearing(
        Dictionary<IElementHandle, ScrapedAdvertisement> dict
    )
    {
        foreach (var pair in dict)
        {
            await using (pair.Key)
            {
                Option<IElementHandle> preview = await pair.Key.GetChildWithClassFormatter(
                    imagePreviewClassName
                );
                if (!preview.HasValue)
                    continue;
                await using IElementHandle previewElement = preview.Value;
                await previewElement.HoverAsync();
            }
        }
    }

    private static async Task InitializeAdvertisementsImages(
        Dictionary<IElementHandle, ScrapedAdvertisement> dict
    )
    {
        foreach (var pair in dict)
        {
            Option<IElementHandle> preview = await pair.Key.GetChildWithClassFormatter(
                imagePreviewClassName
            );
            if (!preview.HasValue)
                continue;

            await using IElementHandle previewElement = preview.Value;
            Option<IElementHandle> imagesContainer =
                await previewElement.GetChildWithClassFormatter(
                    string.Intern("css-1h0gd61 e1lm3vns0")
                );
            if (!imagesContainer.HasValue)
                continue;

            await using IElementHandle imagesContainerElement = imagesContainer.Value;
            Option<IElementHandle[]> images =
                await imagesContainerElement.GetChildrenWithoutClassFormatter("img");
            if (!images.HasValue)
                continue;

            IElementHandle[] imageElements = images.Value;
            List<string> photos = [];
            foreach (var imageElement in imageElements)
            {
                await using (imageElement)
                {
                    Option<string> srcSets = await imageElement.GetElementAttributeValue("srcset");
                    if (!srcSets.HasValue)
                        continue;
                    string[] sets = [.. srcSets.Value.Split(' ').Where(s => s.Contains("http"))];
                    string maxImageSize = sets[^1];
                    photos.Add(maxImageSize);
                }
            }
            pair.Value.PhotoUrls = [.. photos];
        }
    }

    private async Task InitializeInfoFromTitle(
        Dictionary<IElementHandle, ScrapedAdvertisement> dict
    )
    {
        foreach (var pair in dict)
        {
            Option<IElementHandle> titleElement = await pair.Key.GetChildWithClassFormatter(
                "g6gv8w4 g6gv8w8 _1ioeqy90"
            );
            if (!titleElement.HasValue)
                continue;

            await using IElementHandle title = titleElement.Value;
            Option<string> titleText = await title.GetElementText();
            if (!titleText.HasValue)
                continue;

            Option<string> hrefAttribute = await title.GetElementAttributeValue("href");
            if (!hrefAttribute.HasValue)
                continue;
            string titleValue = titleText.Value;
            string urlValue = hrefAttribute.Value;

            ScrapedCharacteristic[] characteristics = new ScrapedCharacteristic[2];
            pair.Value.Title = titleValue;
            pair.Value.SourceUrl = urlValue;
            characteristics[0] = new ScrapedCharacteristic
            {
                Name = "Год выпуска",
                Value = titleValue.Split(',', StringSplitOptions.TrimEntries)[^1],
            };

            Option<IElementHandle> transportTypeElement = await pair.Key.GetChildWithClassFormatter(
                "css-1hd50jd e3f4v4l0"
            );
            if (!transportTypeElement.HasValue)
                continue;
            await using IElementHandle transportTypeElementValue = transportTypeElement.Value;
            Option<string> transportTypeText = await transportTypeElementValue.GetElementText();
            if (!transportTypeText.HasValue)
                continue;
            characteristics[1] = new() { Name = "Тип техники", Value = transportTypeText.Value };
            pair.Value.Characteristics = characteristics;
        }
    }

    private static void InitializeIds(Dictionary<IElementHandle, ScrapedAdvertisement> dict)
    {
        foreach (var pair in dict)
        {
            string url = pair.Value.SourceUrl;
            string id = url.Split('/')[^1].Replace(".html", "");
            pair.Value.AdvertisementId = long.Parse(id);
        }
    }

    private async Task AppendInContext(Dictionary<IElementHandle, ScrapedAdvertisement> dict)
    {
        foreach (var pair in dict)
        {
            await using (pair.Key)
            {
                ScrapedAdvertisement advertisement = pair.Value;
                _context.AddAdvertisement(advertisement);
            }
        }
    }

    private string CreatePageUrl(string currentUrl, ref int pageNum)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(currentUrl);
        stringBuilder.Append($"page{pageNum}/");
        return stringBuilder.ToString();
    }
}
