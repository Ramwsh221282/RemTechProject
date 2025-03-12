using System.Diagnostics;
using System.Text;
using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace DromParser.Tests;

public sealed class Parse_Drom_Advertisement_Tests
{
    private readonly IServiceProvider _provider;

    public Parse_Drom_Advertisement_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        services.InjectPluginPDK();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Scrape_Drom_Catalogue_Tests()
    {
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        string inputLink = "https://auto.drom.ru/spec/bull/loader/all";

        PuppeteerLaunchOptions launchOptions = new PuppeteerLaunchOptions(
            Headless: false,
            Arguments: ["--start-maximized"]
        );

        Result<IBrowser> browserInstantiation = await context.Execute<IBrowser>(
            "InstantiateDromScrapingBrowserPlugin",
            new PluginPayload(logger, context, launchOptions)
        );

        Assert.True(browserInstantiation.IsSuccess);

        await using IBrowser browser = browserInstantiation.Value;
        await Task.Delay(TimeSpan.FromSeconds(5));

        Result<IEnumerable<ScrapedAdvertisement>> ads = await context.Execute<
            IEnumerable<ScrapedAdvertisement>
        >(
            "CollectAdvertisementsDataFromCataloguePlugin",
            new PluginPayload(logger, context, browser, ScrapedSourceUrl.Create(inputLink).Value)
        );
        Assert.True(ads.IsSuccess);

        ScrapedAdvertisement[] advertisements = ads.Value.ToArray();
        Assert.NotNull(advertisements);
    }
}
