using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.BrowserCreation;

public static class BrowserFactory
{
    private static readonly string BrowserCatalogueDefaultDirectory = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Chrome"
    );

    public static async Task<IBrowser> CreateStealthBrowserInstance(bool headless = false)
    {
        await LoadPuppeteerIfNotExists();

        LaunchOptions options = CreateLaunchOptions(headless);
        PuppeteerExtra extra = new();
        StealthPlugin stealth = new();

        extra = extra.Use(stealth);

        IBrowser browser = await extra.LaunchAsync(options);
        return browser;
    }

    private static async Task LoadPuppeteerIfNotExists()
    {
        if (Directory.Exists(BrowserCatalogueDefaultDirectory))
            return;

        using BrowserFetcher fetcher = new();
        await fetcher.DownloadAsync();
    }

    private static LaunchOptions CreateLaunchOptions(bool headless) =>
        new()
        {
            Headless = headless,
            Args = ["--start-maximized"],
            DefaultViewport = new ViewPortOptions
            {
                Width = 1920,
                Height = 1080,
                DeviceScaleFactor = 1,
            },
        };
}
