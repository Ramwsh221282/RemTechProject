using AvitoParser.PDK.Models.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.Tests.Avito_Parser_Plugins_Tests;

public class AvitoParserPluginsTests
{
    private readonly IServiceProvider _provider;

    public AvitoParserPluginsTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        services.InjectPluginPDK();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Get_List_Of_Advertisement_Urls()
    {
        bool noExceptions = true;
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        try
        {
            Result<IBrowser> browserResult = await context.ExecutePlugin<IBrowser>(
                new PluginCommand(
                    "CreateAvitoScraperBrowserInstancePlugin",
                    new PluginPayload(logger, context)
                )
            );
            Assert.True(browserResult.IsSuccess);
            await using IBrowser browser = browserResult.Value;

            ScrapedSourceUrl url = ScrapedSourceUrl.Create(catalogueUrl);
            Result<IPage> pageResult = await context.ExecutePlugin<IPage>(
                new PluginCommand(
                    "CreateAvitoCataloguePagePlugin",
                    new PluginPayload(logger, context, browser, url)
                )
            );
            Assert.True(pageResult.IsSuccess);
            await using IPage page = pageResult.Value;

            Result<IEnumerable<ScrapedSourceUrl>> advertisementLinksResult =
                await context.ExecutePlugin<IEnumerable<ScrapedSourceUrl>>(
                    new PluginCommand(
                        "CreateAvitoAdvertisementLinksListPlugin",
                        new PluginPayload(logger, context, page)
                    )
                );
            IEnumerable<ScrapedSourceUrl> urls = advertisementLinksResult.Value;
            foreach (ScrapedSourceUrl urlResult in urls)
            {
                logger.Information("Url: {Url}", urlResult.SourceUrl);
            }

            Assert.True(advertisementLinksResult.IsSuccess);
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Text} failed. Exception: {Ex}",
                nameof(Get_List_Of_Advertisement_Urls),
                ex.Message
            );
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }
}
