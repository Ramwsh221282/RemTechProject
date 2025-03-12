using System.Reflection;
using CollectAdvertisementsDataFromCataloguePlugin.Models;
using PuppeteerSharp;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class DromPageExtensions
{
    private const string imagePreviewClassName = "emt6rd1";

    public static async Task<Result<DromItemsContainer>> CreateItemsContainer(
        this DromCataloguePage catalogue
    )
    {
        IPage page = catalogue.Page;

        IElementHandle? element = await page.QuerySelectorAsync(DromItemsContainer.ClassSelector);
        if (element == null)
            return new Error("Cannot get catalogue page html element");

        await element.InvokeImageAppearing();

        element = await page.QuerySelectorAsync(DromItemsContainer.ClassSelector);
        if (element == null)
            return new Error("Cannot get catalogue page html element");

        return CreateUsingReflection(element);
    }

    private static async Task InvokeImageAppearing(this IElementHandle element)
    {
        QuerySelectorPayload itemsPayload = new QuerySelectorPayload(DromItem.ClassSelector);
        IElementHandle[] subElements = await element.QuerySelectorAllAsync(itemsPayload.Selector);
        QuerySelectorPayload previewPayload = new QuerySelectorPayload(imagePreviewClassName);
        foreach (IElementHandle subElement in subElements)
        {
            IElementHandle? preview = await subElement.QuerySelectorAsync(previewPayload.Selector);
            if (preview == null)
                continue;
            await preview.HoverAsync();
        }
    }

    private static DromItemsContainer CreateUsingReflection(IElementHandle element)
    {
        Type type = typeof(DromItemsContainer);
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        ConstructorInfo constructor = constructors[0];
        DromItemsContainer container = (DromItemsContainer)constructor.Invoke([element]);
        return container;
    }
}
