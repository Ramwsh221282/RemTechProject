using System.Reflection;
using AvitoParser.PDK.Models;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using Serilog;

namespace CompleteDromAdvertisementsPlugin.Models;

public sealed record AdvertisementPage : IDisposable, IAsyncDisposable
{
    public IPage Page { get; }

    private AdvertisementPage(IPage page) => Page = page;

    public async ValueTask DisposeAsync() => await Page.DisposeAsync();

    public void Dispose() => Page.Dispose();
}

public static class AdvertisementPageFactory
{
    public static async Task<AdvertisementPage> Create(
        IBrowser browser,
        ScrapedAdvertisement fromCatalogue,
        PluginExecutionContext context,
        ILogger logger
    )
    {
        IPage page = await browser.NewPageAsync();
        string url = fromCatalogue.SourceUrl.SourceUrl;
        await page.GoToAsync(url);

        await context.Execute("ScrollBottomPlugin", new PluginPayload(logger, page));
        await context.Execute("ScrollTopPlugin", new PluginPayload(logger, page));

        return page.CreateByReflection();
    }

    private static AdvertisementPage CreateByReflection(this IPage page)
    {
        Type type = typeof(AdvertisementPage);
        ConstructorInfo[] constructors = type.GetConstructors(
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        ConstructorInfo constructor = constructors[0];
        return (AdvertisementPage)constructor.Invoke([page]);
    }
}
