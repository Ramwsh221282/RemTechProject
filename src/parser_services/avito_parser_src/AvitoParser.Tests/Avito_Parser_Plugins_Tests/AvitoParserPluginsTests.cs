using System.Text.Json;
using AvitoParser.PDK.Dtos;
using AvitoParser.PDK.Models.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.Tests.Avito_Parser_Plugins_Tests;

public class AvitoParserPluginsTests
{
    private readonly IServiceProvider _provider;
    private readonly LoginDto _loginData;

    public AvitoParserPluginsTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton<ILogger>(new LoggerConfiguration().WriteTo.Console().CreateLogger());
        services.InjectPluginPDK();
        _provider = services.BuildServiceProvider();

        string loginTextContent = File.ReadAllText("auth.json");
        using JsonDocument jsonDocument = JsonDocument.Parse(loginTextContent);
        string emailValue = jsonDocument.RootElement.GetProperty("email").GetString()!;
        string passwordValue = jsonDocument.RootElement.GetProperty("password").GetString()!;
        _loginData = new LoginDto(emailValue, passwordValue);
    }

    [Fact]
    public async Task Auth_Avito_Test()
    {
        bool noExceptions = true;
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        Result<IBrowser> browserResult = await context.ExecutePlugin<IBrowser>(
            new PluginCommand(
                "CreateAvitoScraperBrowserInstancePlugin",
                new PluginPayload(logger, context)
            )
        );
        Assert.True(browserResult.IsSuccess);
        await using IBrowser browser = browserResult.Value;
        try
        {
            Result<IPage> authPage = await context.ExecutePlugin<IPage>(
                new PluginCommand(
                    "LoginInAvitoPlugin",
                    new PluginPayload(logger, browser, _loginData)
                )
            );
            Assert.True(authPage.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            logger.Fatal("{Test} failed. Exception: {Ex}", nameof(Auth_Avito_Test), ex.Message);
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Parse_Advertisement_Catalogue_Page()
    {
        bool noExceptions = true;
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        const string catalogueUrl =
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0";

        Result<IBrowser> browserResult = await context.ExecutePlugin<IBrowser>(
            new PluginCommand(
                "CreateAvitoScraperBrowserInstancePlugin",
                new PluginPayload(logger, context)
            )
        );
        Assert.True(browserResult.IsSuccess);
        await using IBrowser browser = browserResult.Value;
        try
        {
            Result<IPage> authPage = await context.ExecutePlugin<IPage>(
                new PluginCommand(
                    "LoginInAvitoPlugin",
                    new PluginPayload(
                        logger,
                        browser,
                        new LoginDto("jimkrauz@gmail.com", "9595330zxzO!")
                    )
                )
            );
            Assert.True(authPage.IsSuccess);

            ScrapedSourceUrl url = ScrapedSourceUrl.Create(catalogueUrl);
            Result<IPage> pageResult = await context.ExecutePlugin<IPage>(
                new PluginCommand(
                    "CreateAvitoCataloguePagePlugin",
                    new PluginPayload(logger, context, browser, url)
                )
            );
            Assert.True(pageResult.IsSuccess);
            IPage page = pageResult.Value;

            Result<IEnumerable<ScrapedSourceUrl>> advertisementLinksResult =
                await context.ExecutePlugin<IEnumerable<ScrapedSourceUrl>>(
                    new PluginCommand(
                        "CreateAvitoAdvertisementLinksListPlugin",
                        new PluginPayload(logger, context, page)
                    )
                );
            IEnumerable<ScrapedSourceUrl> urls = advertisementLinksResult.Value;
            await page.DisposeAsync();

            List<Task> tasks = [];
            foreach (var scrapedUrl in urls)
            {
                if (tasks.Count == 4)
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                tasks.Add(
                    Task.Run(async () =>
                    {
                        Result<IPage> advertisementPageResult = await context.ExecutePlugin<IPage>(
                            new PluginCommand(
                                "OpenAdvertisementPagePlugin",
                                new PluginPayload(logger, browser, scrapedUrl, context)
                            )
                        );
                        IPage advertisementPage = advertisementPageResult.Value;

                        Result<ScrapedTitle> title = await context.ExecutePlugin<ScrapedTitle>(
                            new PluginCommand(
                                "CreateAdvertisementTitlePlugin",
                                new PluginPayload(logger, advertisementPage)
                            )
                        );

                        Result<ScrapedPrice> priceResult = await context.Execute<ScrapedPrice>(
                            "CreateAvitoAdvertisementPricePlugin",
                            new PluginPayload(logger, advertisementPage, context)
                        );

                        Result<IReadOnlyCollection<ScrapedCharacteristic>> characteristics =
                            await context.Execute<IReadOnlyCollection<ScrapedCharacteristic>>(
                                "CreateAvitoAdvertisementCharacteristics",
                                new PluginPayload(logger, advertisementPage)
                            );

                        Result<ScrapedAddress> addressResult =
                            await context.Execute<ScrapedAddress>(
                                "CreateAdvertisementAddressPlugin",
                                new PluginPayload(advertisementPage, logger)
                            );

                        Result<ScrapedDescription> descriptionResult =
                            await context.Execute<ScrapedDescription>(
                                "CreateAdvertisementDescriptionPlugin",
                                new PluginPayload(logger, advertisementPage)
                            );

                        Result<IReadOnlyCollection<ScrapedPhotoUrl>> photosResult =
                            await context.Execute<IReadOnlyCollection<ScrapedPhotoUrl>>(
                                "CreateAdvertisementPhotoList",
                                new PluginPayload(logger, advertisementPage)
                            );

                        Result<ScrapedAdvertisementDate> dateResult =
                            await context.Execute<ScrapedAdvertisementDate>(
                                "CreateAdvertisementDatePlugin",
                                new PluginPayload(logger, advertisementPage)
                            );

                        Result<ScrapedPublisher> publisherResult =
                            await context.Execute<ScrapedPublisher>(
                                "CreatePublisherPlugin",
                                new PluginPayload(logger, advertisementPage)
                            );

                        if (publisherResult.IsSuccess)
                            logger.Information("Publisher: {Text}", publisherResult.Value);
                        else
                            logger.Error(
                                "Publisher Error: {Error}",
                                publisherResult.Error.Description
                            );

                        await advertisementPage.DisposeAsync();
                    })
                );
            }

            Assert.True(advertisementLinksResult.IsSuccess);
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Text} failed. Exception: {Ex}",
                nameof(Parse_Advertisement_Catalogue_Page),
                ex.Message
            );
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Parse_Advertisement_Photos()
    {
        const string url =
            "https://www.avito.ru/kazan/gruzoviki_i_spetstehnika/frontalnyy_pogruzchik_linyi_kater_zl-35_2014_4841604438?context=H4sIAAAAAAAA_wEmANn_YToxOntzOjE6IngiO3M6MTY6IjRPMXNaNEVhUHZTWU81WEYiO33Xd6MpJgAAAA";
        const string currentImageContainerSelector = "image-frame-wrapper-_NvbY";
        const string imageListContainerSelector =
            "images-preview-previewWrapper-R_a4U images-preview-previewWrapper_newStyle-fGdrG";

        ViewPortOptions viewPortOptions = new ViewPortOptions()
        {
            Width = 1920,
            Height = 1080,
            DeviceScaleFactor = 1,
        };

        LaunchOptions launchOptions = new LaunchOptions()
        {
            Headless = false,
            Args = ["--start-maximized"],
            DefaultViewport = viewPortOptions,
        };

        PuppeteerExtra extra = new PuppeteerExtra();
        StealthPlugin stealthPlugin = new StealthPlugin();
        extra = extra.Use(stealthPlugin);
        await using IBrowser browser = await extra.LaunchAsync(launchOptions);

        await using IPage page = await browser.NewPageAsync();
        await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);

        List<string> Photos = [];

        IElementHandle currentImage = await page.QuerySelectorAsync(
            new QuerySelectorPayload(currentImageContainerSelector).Selector
        );
        Assert.NotNull(currentImage);

        IElementHandle imagesList = await page.QuerySelectorAsync(
            new QuerySelectorPayload(imageListContainerSelector).Selector
        );
        Assert.NotNull(imagesList);

        IElementHandle[] imagesArray = await imagesList.QuerySelectorAllAsync("li");
        Assert.NotEmpty(imagesArray);

        IElementHandle currentImg = await currentImage.QuerySelectorAsync("img");
        Assert.NotNull(currentImg);

        Photos.Add(await currentImg.EvaluateFunctionAsync<string>($"el => el.getAttribute('src')"));
        for (int index = 1; index < imagesArray.Length; index++)
        {
            string dataType = await imagesArray[index]
                .EvaluateFunctionAsync<string>("el => el.getAttribute('data-type')");
            if (dataType != "image")
                continue;
            await imagesArray[index].ClickAsync();
            Photos.Add(
                await currentImg.EvaluateFunctionAsync<string>($"el => el.getAttribute('src')")
            );
        }

        Assert.NotEmpty(Photos);
    }
}
