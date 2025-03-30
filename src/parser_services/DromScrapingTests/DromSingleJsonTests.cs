using System.Text.Json;
using DromScrapingTests.DromDeserializationModels;
using PuppeteerSharp;
using RemTechCommon.Utils.OptionPattern;
using SharedParsersLibrary.Models;
using SharedParsersLibrary.Puppeteer.Features.BrowserCreation;

namespace DromScrapingTests;

public sealed class BrowsingContext : IDisposable
{
    private bool _isDisposed;

    public IBrowser Instance { get; }

    private BrowsingContext(IBrowser browser)
    {
        Instance = browser;
        _isDisposed = false;
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        Instance.Dispose();
        _isDisposed = true;
    }

    public static async Task<BrowsingContext> Create()
    {
        BrowserFactory factory = new BrowserFactory();
        await factory.LoadPuppeteerIfNotExists();
        IBrowser browser = await factory.CreateStealthBrowserInstance();
        return new BrowsingContext(browser);
    }

    public async Task<Option<DromDataScriptElement>> TryExtractDromDataScriptElement(IPage page)
    {
        string selector = string.Intern("[data-drom-module]");
        await page.WaitForSelectorAsync(selector);
        IElementHandle[] elements = await page.QuerySelectorAllAsync(selector);

        foreach (IElementHandle item in elements)
        {
            string innerHtml = await item.EvaluateFunctionAsync<string>("el => el.innerHTML");
            if (innerHtml.Contains("header", StringComparison.OrdinalIgnoreCase))
                return Option<DromDataScriptElement>.Some(new DromDataScriptElement(innerHtml));
        }

        return Option<DromDataScriptElement>.None();
    }
}

public sealed record DromDataScriptElement(string InnerHtml);

public static class DromDataScriptBehavior
{
    public static JsonDocument CreateJsonDocument(this DromDataScriptElement element)
    {
        return JsonDocument.Parse(element.InnerHtml);
    }
}

public class DromSingleJsonTests
{
    [Fact]
    public async Task GetJson()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();
    }

    [Fact]
    public async Task ParseJson()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        document.Dispose();
    }

    [Fact]
    public async Task ExtractFields()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        Assert.True(document.RootElement.TryGetProperty("bullDescription", out JsonElement _));
        Assert.True(
            document.RootElement.TryGetProperty("additionalCandyConfig", out JsonElement _)
        );
        Assert.True(document.RootElement.TryGetProperty("bullCounters", out JsonElement _));
        Assert.True(document.RootElement.TryGetProperty("pageMeta", out JsonElement _));
        Assert.True(document.RootElement.TryGetProperty("constants", out JsonElement _));
    }

    [Fact]
    public async Task ParseBullDescriptionNode()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        bool noExceptions = true;

        try
        {
            DromTransportAttributeTypeFactory.CreateFromJsonDocument(document);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task ParseBullCounters()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        bool noExceptions = true;

        try
        {
            BullCountersFactory.FromJsonDocument(document);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task ParseAdditionalCandyConfig()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        bool noExceptions = true;

        try
        {
            AdditionalCandyConfigFactory.FromJsonDocument(document);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task ParseDealerOrPrivatePerson()
    {
        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(
            "https://auto.drom.ru/spec/vladivostok/bull/sl1050/loader/mini/45927976.html",
            WaitUntilNavigation.DOMContentLoaded
        );
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        bool noExceptions = true;

        try
        {
            GeoInfoFactory.FromJsonDocument(document);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task ParseAdditionalInfo()
    {
        string url = "https://auto.drom.ru/spec/kursk/bull/412r/loader/mini/941361458.html";

        BrowsingContext context = await BrowsingContext.Create();
        IBrowser instance = context.Instance;
        IPage page = await instance.NewPageAsync();
        await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);
        Option<DromDataScriptElement> scriptElement = await context.TryExtractDromDataScriptElement(
            page
        );
        Assert.True(scriptElement.HasValue);
        context.Dispose();

        DromDataScriptElement script = scriptElement.Value;
        JsonDocument document = JsonDocument.Parse(script.InnerHtml);
        bool noExceptions = true;

        try
        {
            AdditionalInfo info = AdditionalInfoFactory.FromJsonDocument(document);
            ScrapedAdvertisement advertisement = new ScrapedAdvertisement();
            advertisement = info.Set(advertisement);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }
}
