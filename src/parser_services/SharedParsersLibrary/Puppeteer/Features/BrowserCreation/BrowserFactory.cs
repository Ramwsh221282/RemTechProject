using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.BrowserCreation;

public sealed class BrowserFactory
{
    private static readonly string BrowserCatalogueDefaultDirectory = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Chrome"
    );

    public async Task<bool> LoadPuppeteerIfNotExists()
    {
        if (Directory.Exists(BrowserCatalogueDefaultDirectory))
            return false;

        using BrowserFetcher fetcher = new BrowserFetcher();
        await fetcher.DownloadAsync();
        return true;
    }

    public async Task<IBrowser> CreateStealthBrowserInstance()
    {
        LaunchOptions options = CreateLaunchOptions();
        PuppeteerExtra extra = new PuppeteerExtra();
        StealthPlugin stealth = new StealthPlugin();
        extra = extra.Use(stealth);
        IBrowser browser = await extra.LaunchAsync(options);
        return browser;
    }

    private LaunchOptions CreateLaunchOptions() =>
        new LaunchOptions()
        {
            Headless = false,
            Args = ["--start-maximized"],
            DefaultViewport = new ViewPortOptions
            {
                Width = 1920,
                Height = 1080,
                DeviceScaleFactor = 1,
            },
        };
}
