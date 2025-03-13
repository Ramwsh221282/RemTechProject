using System.Text.Json;
using AvitoParser.PDK;
using AvitoParser.PDK.Dtos;
using AvitoParser.PDK.Models;
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
    public async Task Parse_All_Catalogue_Pages()
    {
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();
        ILogger logger = _provider.GetRequiredService<ILogger>();
        ScrapedSourceUrl catalogue = ScrapedSourceUrl.Create(
            "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki/lugong-ASgBAgICAkRU4E3cxg2y0mY?cd=1"
        );
        Result<IEnumerable<ScrapedAdvertisement>> advertisements = await context.Execute<
            IEnumerable<ScrapedAdvertisement>
        >(
            "ParseAvitoCatalogueByUrlPlugin",
            new PluginPayload(context, logger, catalogue, _loginData)
        );
        int bpoint = 0;
        Assert.True(advertisements.IsSuccess);
    }
}
