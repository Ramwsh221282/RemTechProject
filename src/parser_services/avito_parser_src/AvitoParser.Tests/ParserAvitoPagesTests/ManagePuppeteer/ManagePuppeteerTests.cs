using System.Diagnostics;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using Serilog;
using ILogger = Serilog.ILogger;

namespace AvitoParser.Tests.ParserAvitoPagesTests.ManagePuppeteer;

public sealed record ScraperOptionsFactory
{
    public LaunchOptions Create(bool headless = false,
        int width = 1920,
        int height = 1080,
        int deviceScaleFactor = 1)
    {
        ViewPortOptions viewPortOptions = new ViewPortOptions()
        {
            Width = width,
            Height = height,
            DeviceScaleFactor = deviceScaleFactor,
        };
        LaunchOptions launchOptions = new LaunchOptions
        {
            DefaultViewport = viewPortOptions,
            Headless = headless
        };

        return launchOptions;
    }
}

public sealed record ScraperInstaller
{
    private readonly ILogger _logger;

    public ScraperInstaller(ILogger logger) => _logger = logger;

    public async Task<string> InstallScraper()
    {
        _logger.Information("Start downloading Puppeteer.");
        BrowserFetcher fetcher = new BrowserFetcher();
        Task<InstalledBrowser> loadingProcess = fetcher.DownloadAsync();
        Stopwatch sw = Stopwatch.StartNew();
        _logger.Information("Puppeteer loading has been started");
        while (loadingProcess.IsCompleted == false)
            _logger.Information("Loading puppeteer... {seconds} seconds", sw.Elapsed.Seconds);
        InstalledBrowser installedBrowser = loadingProcess.Result;
        string executablePath = installedBrowser.GetExecutablePath();
        _logger.Information("Puppeteer has loaded browser.");
        _logger.Information("Executable path: {Path}", executablePath);
        return executablePath;
    }
}

public sealed record ScraperInstanceFactory(ILogger logger)
{
    public async Task<IBrowser> Create()
    {
        bool exists = Directory.Exists("Chrome");
        if (exists)
        {
            return await Instantiate();
        }
        else
        {
        }
    }

    private async Task<IBrowser> Instantiate()
    {
        var extra = new PuppeteerExtra();
        var stealth = new StealthPlugin();
        LaunchOptions
        extra.Use(stealth);
        return (await extra.LaunchAsync(options));
    }
}

public class ManagePuppeteerTests
{
    private readonly ILogger _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

    [Fact]
    public async Task Download_Puppeteer()
    {
        BrowserFetcher fetcher = new BrowserFetcher();
        _logger.Information("Start downloading Puppeteer.");
        var installed = await fetcher.DownloadAsync();
        _logger.Information("Finish downloading Puppeteer.");
        string executablePath = installed.GetExecutablePath();
        _logger.Information("Executable path: {Path}", executablePath);
        Assert.NotNull(executablePath);
        Assert.NotEmpty(executablePath);
        Assert.True(File.Exists(executablePath));
        // creates Chrome directory in application folder.
    }

    [Fact]
    public async Task Start_Puppeteer_Browser()
    {
        var extra = new PuppeteerExtra();
        var stealth = new StealthPlugin();
        LaunchOptions options = new LaunchOptions
        {
            DefaultViewport = new ViewPortOptions()
            {
                Width = 1920,
                Height = 1080,
                DeviceScaleFactor = 1,
            },
            Headless = false,
        };
        await using IBrowser browser = await extra.Use(stealth).LaunchAsync(options);
        var page = await browser.NewPageAsync();
        await Task.Delay(TimeSpan.FromSeconds(10));
        await page.GoToAsync("https://bot.sannysoft.com/");
        await Task.Delay(TimeSpan.FromSeconds(10));
    }
}