using AvitoParser.PDK;
using Microsoft.Extensions.DependencyInjection;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.Tests.ParserAvitoPagesTests.ManagePuppeteer;

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
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();

        PluginCommand loadPuppeteerCommand = new PluginCommand(
            "LoadPuppeteerPlugin",
            new AvitoPluginPayload(logger)
        );

        Result result = await context.ExecutePlugin(loadPuppeteerCommand);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Instantiate_Puppeteer_And_Stop_Test()
    {
        ILogger logger = _provider.GetRequiredService<ILogger>();
        PluginExecutionContext context = _provider.GetRequiredService<PluginExecutionContext>();

        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments: ["--start-maximized"],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new AvitoPluginPayload(options, logger)
        );

        Result<IBrowser> instantiation = await context.ExecutePlugin<IBrowser>(instantiateCommand);
        Assert.True(instantiation.IsSuccess);
        IBrowser browser = instantiation.Value;
        IPage page = await browser.NewPageAsync();
        var response = await page.GoToAsync("https://bot.sannysoft.com/");
        await Task.Delay(TimeSpan.FromSeconds(5));
        await page.DisposeAsync();
        await browser.DisposeAsync();
    }
}
