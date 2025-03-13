using System.Reflection;
using CollectAdvertisementsDataFromCataloguePlugin.Models;
using PuppeteerSharp;
using RemTech.Puppeteer.Scraper.Plugin.PDK;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class DromCatalogueContainerExtensions
{
    public static async Task<DromItem[]> ExtractAdvertisementsFromCatalogue(
        this DromItemsContainer container
    )
    {
        IElementHandle element = container.Element;
        QuerySelectorPayload payload = new QuerySelectorPayload(DromItem.ClassSelector);
        IElementHandle[] subElements = await element.QuerySelectorAllAsync(payload.Selector);
        return subElements.CreateEmptyDromItemsArray();
    }

    private static DromItem[] CreateEmptyDromItemsArray(this IElementHandle[] elements)
    {
        DromItem[] dromItems = new DromItem[elements.Length];
        for (int i = 0; i < elements.Length; i++)
            dromItems[i] = elements[i].CreateEmptyDromItemByReflection();
        return dromItems;
    }

    private static DromItem CreateEmptyDromItemByReflection(this IElementHandle element)
    {
        Type type = typeof(DromItem);
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        ConstructorInfo constructor = constructors[0];
        return (DromItem)constructor.Invoke([element]);
    }
}
