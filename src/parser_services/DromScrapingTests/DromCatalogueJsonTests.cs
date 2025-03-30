using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedParsersLibrary.Contracts;

namespace DromScrapingTests;

public class DromCatalogueJsonTests
{
    private readonly IServiceProvider _provider;

    public DromCatalogueJsonTests()
    {
        IServiceCollection collection = new ServiceCollection();
        collection.RegisterLogger();
        collection.RegisterConcreteHandler();
        collection.RegisterCatalogueHandler();
        _provider = collection.BuildServiceProvider();
    }

    [Fact]
    public async Task Scrape()
    {
        string[] urls =
        [
            "https://auto.drom.ru/spec/liugong/loader/all/",
            "https://auto.drom.ru/spec/bull/loader/all/",
            "https://auto.drom.ru/spec/sdlg/loader/all/",
        ];
        foreach (string url in urls)
        {
            ScrapeAdvertisementCommand command = new(url);
            IScrapeAdvertisementsHandler handler =
                _provider.GetRequiredService<IScrapeAdvertisementsHandler>();
            await handler.Handle(command);
        }
    }

    [Fact]
    public void RemoveAllChromeProcesses()
    {
        foreach (Process process in Process.GetProcessesByName("chrome"))
        {
            process.Kill();
        }
    }
}

public static class DromCatalogueJsonTestsInjections
{
    public static IServiceCollection RegisterLogger(this IServiceCollection services)
    {
        ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        services.AddSingleton(logger);
        return services;
    }

    public static IServiceCollection RegisterConcreteHandler(this IServiceCollection services)
    {
        // services.AddTransient<IScrapeConcreteAdvertisementHandler>(_ =>
        // {
        //     ScrapeConcreteAdvertisementByJsonContext context = new();
        //     ScrapeConcreteAdvertisementByJsonCoreHandler h1 = new(context);
        //     ScrapeConcreteAdvertisementByJsonPriceExtraDecorator h2 = new(h1, context);
        //     ScrapeConcreteAdvertisementByJsonInitializerDecorator h3 = new(h2, context);
        //     return h3;
        // });
        return services;
    }

    public static IServiceCollection RegisterCatalogueHandler(this IServiceCollection services)
    {
        // services.AddTransient<IScrapeAdvertisementsHandler>(p =>
        // {
        //     ILogger logger = p.GetRequiredService<ILogger>();
        //     DromCatalogueScrapingContext context = new();
        //     DromScrapeAdvertisementsByJsonCoreHandler h1 = new(context, p);
        //     DromScrapeCataloguePagesByJsonDecorator h2 = new(h1, context, logger);
        //     DromScrapeAdvertisementsByJsonInitializeDecorator h3 = new(h2, context);
        //     return h3;
        // });
        return services;
    }
}
