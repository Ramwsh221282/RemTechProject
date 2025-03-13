using PuppeteerSharp;

namespace CollectAdvertisementsDataFromCataloguePlugin.Extensions;

public static class IElementExtensions
{
    private const string textScript = "el => el.textContent";

    public static async Task<string?> ExtractAttribute(
        this IElementHandle element,
        string attributeName
    ) => await element.EvaluateFunctionAsync<string>($"el => el.getAttribute('{attributeName}')");

    public static async Task<string?> ExtractText(this IElementHandle element) =>
        await element.EvaluateFunctionAsync<string>(textScript);
}
