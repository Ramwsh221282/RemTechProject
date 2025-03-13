using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using CollectAdvertisementsDataFromCataloguePlugin.Models;
using PuppeteerSharp;
using RemTech.Puppeteer.Scraper.Plugin.PDK;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class DromItemTitleExtensions
{
    private const string titleClassName = "g6gv8w4 g6gv8w8 _1ioeqy90";
    private const string photoContainerClassName = "css-1h0gd61 e1lm3vns0";
    private const string photoAttribute = "srcset";

    public static async Task<ScrapedAdvertisement> UpdateWithTitleInfo(
        this DromItem item,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload payload = new QuerySelectorPayload(titleClassName);
        string selector = payload.Selector;
        IElementHandle element = item.Element;
        IElementHandle? title = await element.QuerySelectorAsync(selector);

        if (title == null)
            return advertisement;

        string? text = await title.ExtractText();

        if (string.IsNullOrWhiteSpace(text))
            return advertisement;

        string? url = await title.ExtractAttribute("href");
        if (url == null)
            return advertisement;

        string idPart = url.Split('/')[^1].Split('.')[0];

        return advertisement with
        {
            Title = ScrapedTitle.Create(text),
            SourceUrl = ScrapedSourceUrl.Create(url),
            Id = ScrapedAdvertisementId.Create(idPart),
        };
    }

    public static ScrapedAdvertisement UpdateWithCharacteristics(
        this ScrapedAdvertisement advertisement
    )
    {
        ScrapedTitle title = advertisement.Title;
        if (string.IsNullOrWhiteSpace(title.Title))
            return advertisement;

        string titleText = title.Title;
        string[] splitted = titleText.Split(',');
        string modelPart = splitted[0].Trim();
        string yearPart = splitted[1].Trim();

        string[] splittedModelPart = modelPart.Split(' ');

        ScrapedCharacteristic mark = ScrapedCharacteristic.Create("Марка", splittedModelPart[0]);
        ScrapedCharacteristic model = ScrapedCharacteristic.Create("Модель", splittedModelPart[1]);
        ScrapedCharacteristic year = ScrapedCharacteristic.Create("Год выпуска", yearPart);

        List<ScrapedCharacteristic> ctx = [mark, model, year, .. advertisement.Characteristics];
        return advertisement with { Characteristics = ctx };
    }

    public static async Task<ScrapedAdvertisement> UpdateWithPhotos(
        this DromItem item,
        ScrapedAdvertisement advertisement
    )
    {
        IElementHandle itemElement = item.Element;
        QuerySelectorPayload photoContainerPayload = new QuerySelectorPayload(
            photoContainerClassName
        );
        string photoContainerSelector = photoContainerPayload.Selector;
        IElementHandle? photoContainer = await itemElement.QuerySelectorAsync(
            photoContainerSelector
        );
        if (photoContainer == null)
            return advertisement;

        IElementHandle[] photoNodes = await photoContainer.QuerySelectorAllAsync("img");
        List<ScrapedPhotoUrl> photos = [];
        for (int i = 0; i < photoNodes.Length; i++)
        {
            string? srcSet = await photoNodes[i].ExtractAttribute(photoAttribute);
            if (srcSet == null)
                continue;

            string maxSizeImage = srcSet.Split(',')[^1].Trim().Split(' ')[0].Trim();
            photos = [ScrapedPhotoUrl.Create(maxSizeImage), .. photos];
        }

        return advertisement with
        {
            Photos = photos,
        };
    }
}
