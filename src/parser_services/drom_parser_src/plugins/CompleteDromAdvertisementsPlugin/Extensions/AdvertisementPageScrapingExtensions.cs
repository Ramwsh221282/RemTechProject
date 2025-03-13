using System.Globalization;
using System.Text.RegularExpressions;
using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using CompleteDromAdvertisementsPlugin.Models;
using PuppeteerSharp;
using RemTech.Puppeteer.Scraper.Plugin.PDK;

namespace CompleteDromAdvertisementsPlugin.Extensions;

public static class AdvertisementPageScrapingExtensions
{
    private const string priceClassName = "wb9m8q0";
    private const string priceExtraClassName = "css-1oit28e ejipaoe0";
    private const string characteristicsClassName = "css-xalqz7 eo7fo180";
    private const string descriptionClassName = "css-inmjwf e162wx9x0";
    private const string addressContainerClassName = "css-1j8ksy7 eotelyr0";
    private const string addressSpansClassName = "css-inmjwf e162wx9x0";
    private const string dateClassName = "css-pxeubi evnwjo70";
    private const string publisherInfoContainerClassName = "ylnx3y2 css-uewl2b";
    private const string publishInfoNodeClassName = "g6gv8w4 g6gv8w8 ylnx3y4";

    private static WaitForSelectorOptions Options =>
        new WaitForSelectorOptions { Timeout = 300000 };

    public static async Task<ScrapedAdvertisement> ScrapePrice(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload pricePayload = new QuerySelectorPayload(priceClassName);
        QuerySelectorPayload extraPayload = new QuerySelectorPayload(priceExtraClassName);
        string priceSelector = pricePayload.Selector;
        string extraSelector = extraPayload.Selector;

        await page.Page.WaitForSelectorAsync(priceSelector, Options);
        await using IElementHandle? priceElement = await page.Page.QuerySelectorAsync(
            priceSelector
        );
        await using IElementHandle? extraElement = await page.Page.QuerySelectorAsync(
            extraSelector
        );

        if (priceElement == null)
            return advertisement;

        string? priceText = await priceElement.GetText();
        if (string.IsNullOrWhiteSpace(priceText))
            return advertisement;

        priceText = new string(priceText.Where(char.IsDigit).ToArray());
        ScrapedPrice price = (extraElement == null) switch
        {
            true => ScrapedPrice.Create(priceText, "None"),
            false => ScrapedPrice.Create(priceText, await extraElement.GetText() ?? "None"),
        };

        ScrapedAdvertisement updated = advertisement with { Price = price };
        return updated;
    }

    public static async Task<ScrapedAdvertisement> ScrapeCharacteristics(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload characteristicsPayload = new QuerySelectorPayload(
            characteristicsClassName
        );
        string characteristicsSelector = characteristicsPayload.Selector;

        await page.Page.WaitForSelectorAsync(characteristicsSelector, Options);
        await using IElementHandle? characteristicsElement = await page.Page.QuerySelectorAsync(
            characteristicsSelector
        );
        if (characteristicsElement == null)
            return advertisement;

        await using IElementHandle? tbody = await characteristicsElement.QuerySelectorAsync(
            "tbody"
        );
        if (tbody == null)
            return advertisement;

        IElementHandle[] rows = await tbody.QuerySelectorAllAsync("tr");
        List<ScrapedCharacteristic> ctx = [];
        foreach (var row in rows)
        {
            await using (row)
            {
                await using IElementHandle? ctxNameElement = await row.QuerySelectorAsync("th");
                await using IElementHandle? ctxValueElement = await row.QuerySelectorAsync("td");

                if (ctxNameElement == null || ctxValueElement == null)
                    continue;

                string ctxName = (await ctxNameElement.GetText())!;
                string ctxValue = (await ctxValueElement.GetText())!;

                if (ctxValue.Contains('³'))
                    ctxValue = ctxValue.Replace("\u00b3", "");

                if (ctxValue.Contains('*'))
                    ctxValue = ctxValue.Replace("*", "");

                ScrapedCharacteristic ctxItem = ScrapedCharacteristic.Create(ctxName, ctxValue);
                ctx.Add(ctxItem);
            }
        }

        List<ScrapedCharacteristic> updatedCharacteristics =
        [
            .. ctx,
            .. advertisement.Characteristics,
        ];
        ScrapedAdvertisement updated = advertisement with
        {
            Characteristics = updatedCharacteristics,
        };
        return updated;
    }

    public static async Task<ScrapedAdvertisement> ScrapeDescription(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload descriptionPayload = new QuerySelectorPayload(descriptionClassName);
        string descriptionSelector = descriptionPayload.Selector;

        await page.Page.WaitForSelectorAsync(descriptionSelector, Options);
        await using IElementHandle? description = await page.Page.QuerySelectorAsync(
            descriptionSelector
        );

        if (description == null)
            return advertisement;

        string? text = await description.GetText();
        text = text.CleanStringWithRegex();
        if (text.Contains("дополнительно", StringComparison.OrdinalIgnoreCase))
        {
            string[] splitted = text.Split("Дополнительно:", StringSplitOptions.RemoveEmptyEntries);
            text = splitted[^1];
        }

        ScrapedDescription result = ScrapedDescription.Create(text);
        ScrapedAdvertisement updated = advertisement with { Description = result };
        return updated;
    }

    public static async Task<ScrapedAdvertisement> ScrapeAddress(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload addressContainerPayload = new QuerySelectorPayload(
            addressContainerClassName
        );
        string addressContainerSelector = addressContainerPayload.Selector;
        QuerySelectorPayload addressSpansPayload = new QuerySelectorPayload(addressSpansClassName);
        string addressSpansSelector = addressSpansPayload.Selector;

        await page.Page.WaitForSelectorAsync(addressContainerSelector, Options);
        await using IElementHandle? addressContainerElement = await page.Page.QuerySelectorAsync(
            addressContainerSelector
        );

        if (addressContainerElement == null)
            return advertisement;

        IElementHandle[] spans = await addressContainerElement.QuerySelectorAllAsync(
            addressSpansSelector
        );
        string cityText = string.Empty;
        foreach (var span in spans)
        {
            await using (span)
            {
                string? text = await span.GetText();
                if (string.IsNullOrWhiteSpace(text))
                    continue;
                if (!text.Contains("Город:"))
                    continue;
                cityText = text;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(cityText))
            return advertisement;

        string[] splitted = cityText.Split(
            "Город:",
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );

        string cityValue = splitted[^1];
        ScrapedAddress address = ScrapedAddress.Create(cityValue);
        ScrapedAdvertisement updated = advertisement with { Address = address };
        return updated;
    }

    public static async Task<ScrapedAdvertisement> ScrapeDate(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload payload = new QuerySelectorPayload(dateClassName);
        string selector = payload.Selector;

        await page.Page.WaitForSelectorAsync(selector, Options);
        await using IElementHandle? element = await page.Page.QuerySelectorAsync(selector);
        if (element == null)
            return advertisement;

        string? text = await element.GetText();
        if (string.IsNullOrWhiteSpace(text))
            return advertisement;

        string[] splitted = text.Split(
            "от",
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
        );
        string datePart = splitted[^1];

        DateTime date = DateTime.ParseExact(datePart, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        ScrapedAdvertisementDate result = new ScrapedAdvertisementDate(date);
        ScrapedAdvertisement updated = advertisement with { Date = result };
        return updated;
    }

    public static async Task<ScrapedAdvertisement> ScrapePublisher(
        this AdvertisementPage page,
        ScrapedAdvertisement advertisement
    )
    {
        QuerySelectorPayload containerPayload = new QuerySelectorPayload(
            publisherInfoContainerClassName
        );
        string containerSelector = containerPayload.Selector;
        QuerySelectorPayload nodePayload = new QuerySelectorPayload(publishInfoNodeClassName);
        string nodeSelector = nodePayload.Selector;

        await page.Page.WaitForSelectorAsync(containerSelector, Options);
        await using IElementHandle? element = await page.Page.QuerySelectorAsync(containerSelector);
        if (element == null)
            return advertisement;

        IElementHandle? nodeElement = await element.QuerySelectorAsync(nodeSelector);
        if (nodeElement == null)
            return advertisement;

        string? text = await nodeElement.GetText();
        if (string.IsNullOrWhiteSpace(text))
            return advertisement;

        ScrapedPublisher publisher = ScrapedPublisher.Create(text);
        ScrapedAdvertisement updated = advertisement with { Publisher = publisher };
        return updated;
    }

    private static async Task<string?> GetText(this IElementHandle element) =>
        await element.EvaluateFunctionAsync<string>("el => el.textContent");

    public static string CleanStringWithRegex(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        string cleaned = Regex.Replace(input, @"[^\w\s.,!?-]", "");
        cleaned = Regex.Replace(cleaned, @"(\r\n|\n){3,}", "\n");
        return cleaned;
    }
}
