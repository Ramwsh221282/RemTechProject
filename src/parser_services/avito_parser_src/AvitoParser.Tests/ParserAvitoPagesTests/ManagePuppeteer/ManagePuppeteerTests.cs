using System.Reflection;
using AvitoParser.PDK;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.Tests.ParserAvitoPagesTests.ManagePuppeteer;

public sealed record ScraperOptionsFactory
{
    private readonly ILogger _logger;

    public ScraperOptionsFactory(ILogger logger) => _logger = logger;

    public LaunchOptions Create(
        bool headless = false,
        int width = 1920,
        int height = 1080,
        int deviceScaleFactor = 1,
        params string[] arguments
    )
    {
        ViewPortOptions viewPortOptions = new ViewPortOptions
        {
            Width = width,
            Height = height,
            DeviceScaleFactor = deviceScaleFactor,
        };

        _logger.Information(
            "Scraper view port options has been set: Width: {Width} Height: {Height} Device Scale Factor: {Device Scale Factor}",
            width,
            height,
            deviceScaleFactor
        );

        LaunchOptions launchOptions = new LaunchOptions
        {
            DefaultViewport = viewPortOptions,
            Headless = headless,
            Args = arguments,
        };

        _logger.Information("Launch options has been set");
        _logger.Information("Headless {Headless}", headless);
        _logger.Information("Parameters: {Parameters}", string.Join(' ', arguments));
        return launchOptions;
    }
}

public sealed record ScraperInstaller
{
    private readonly ILogger _logger;

    public ScraperInstaller(ILogger logger) => _logger = logger;

    public async Task<Result> InstallScraper()
    {
        _logger.Information("{Context}. Start downloading Puppeteer.", nameof(ScraperInstaller));
        BrowserFetcher fetcher = new BrowserFetcher();
        try
        {
            InstalledBrowser browser = await fetcher.DownloadAsync();
            _logger.Information(
                "{Context}. Puppeteer downloaded. Excutable path: {Path}",
                nameof(ScraperInstaller),
                browser.GetExecutablePath()
            );
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.Information(
                "{Context} failed at downloading Puppeteer. Error: {Message}",
                nameof(ScraperInstaller),
                ex.Message
            );
            return Result.Failure(new Error("Cannot download puppeteer automatic browser"));
        }
    }
}

public sealed record ScraperInstanceFactory
{
    private readonly ScraperInstaller _installer;
    private readonly ILogger _logger;
    private readonly ScraperOptionsFactory _optionsFactory;
    private const string BrowserDirectory = "Chrome";

    public ScraperInstanceFactory(
        ILogger logger,
        ScraperInstaller installer,
        ScraperOptionsFactory factory
    ) => (_logger, _installer, _optionsFactory) = (logger, installer, factory);

    public async Task<Result<IBrowser>> Create(params string[] arguments)
    {
        bool exists = Directory.Exists(BrowserDirectory);
        if (exists)
            return await Instantiate(arguments);

        _logger.Information("Automatic browser executable does not exist. Downloading.");
        Result result = await _installer.InstallScraper();
        if (!result.IsFailure)
            return await Instantiate(arguments);

        Error error = result.Error;
        _logger.Error(
            "{Context}. Can't instantiate new browser instance. Error: {error}",
            nameof(ScraperInstanceFactory),
            error.Description
        );
        return error;
    }

    private async Task<Result<IBrowser>> Instantiate(params string[] arguments)
    {
        PuppeteerExtra extra = new PuppeteerExtra();
        StealthPlugin plugin = new StealthPlugin();
        extra.Use(plugin);
        LaunchOptions options = _optionsFactory.Create(arguments: arguments);
        IBrowser browser = await extra.LaunchAsync(options);
        _logger.Information(
            "{Context}. Instantiated new browser instance",
            nameof(ScraperInstanceFactory)
        );
        return Result<IBrowser>.Success(browser);
    }
}

public class ManagePuppeteerTests
{
    private readonly IServiceProvider _provider;

    public ManagePuppeteerTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        services.AddTransient<PluginExecutionContext>();
        services.AddTransient<PluginFileValidator>();
        services.AddTransient<PluginResolver>();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Load_Puppeteer_Plugin_Test()
    {
        ILogger logger = _provider.GetRequiredService<ILogger>();
        const string pluginPath =
            @"D:\RemTechRework\RemTechProject\src\parser_services\avito_parser_src\Plugins\LoadPuppeteerPlugin\bin\Debug\net9.0\LoadPuppeteerPlugin.dll";
        const string pluginName = "LoadPuppeteerPlugin";
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        Result result = await context.ExecutePlugin(
            pluginPath,
            pluginName,
            new AvitoPluginPayload(logger)
        );
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Instantiate_Puppeteer_And_Stop_Test()
    {
        ILogger logger = _provider.GetRequiredService<ILogger>();
        const string pluginPath =
            @"D:\RemTechRework\RemTechProject\src\parser_services\avito_parser_src\Plugins\InstantiatePuppeteerPlugin\bin\Debug\net9.0\InstantiatePuppeteerPlugin.dll";
        const string pluginName = "InstantiatePuppeteerPlugin";
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );
        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(
            pluginPath,
            pluginName,
            new AvitoPluginPayload(options, logger)
        );
        Assert.True(instantiation.IsSuccess);
        IBrowser browser = instantiation.Value;
        IPage page = await browser.NewPageAsync();
        var response = await page.GoToAsync("https://bot.sannysoft.com/");
        await Task.Delay(TimeSpan.FromSeconds(5));
        await page.DisposeAsync();
        await browser.DisposeAsync();
    }

    // [Fact]
    // public async Task Start_Puppeteer_Browser()
    // {
    //     ScraperInstanceFactory factory = _provider.GetRequiredService<ScraperInstanceFactory>();
    //     Result<IBrowser> instantiation = await factory.Create();
    //     Assert.True(instantiation.IsSuccess);
    //     await using IBrowser browser = instantiation.Value;
    //     var page = await browser.NewPageAsync();
    //     await Task.Delay(TimeSpan.FromSeconds(10));
    //     await page.GoToAsync("https://bot.sannysoft.com/");
    //     await Task.Delay(TimeSpan.FromSeconds(10));
    // }
}

public sealed record DummyRecord(string Name, string Value);
