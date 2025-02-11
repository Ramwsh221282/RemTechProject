using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using WebDriver.Application.Commands.ClickOnElement;
using WebDriver.Application.Commands.OpenPage;
using WebDriver.Application.Commands.ScrollToDown;
using WebDriver.Application.Commands.ScrollToTop;
using WebDriver.Application.Commands.StartWebDriver;
using WebDriver.Application.Commands.StopWebDriver;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Application.Queries.GetElement;
using WebDriver.Application.Queries.GetElementInsideOfElement;
using WebDriver.Application.Queries.GetElementsInsideOfElement;
using WebDriver.Application.Queries.GetPageHtml;
using WebDriver.Application.Queries.GetTextFromElement;
using WebDriver.Core.Models;

namespace WebDriver.Application.Injection;

public static class WebDriverPluginLoader
{
    public static void RegisterWebDriverServices(this IServiceCollection services)
    {
        services.RegisterWebDriverDependencies();
        services.RegisterCommands();
        services.RegisterQueries();
        services.RegisterValidators();
    }

    private static void RegisterCommands(this IServiceCollection services)
    {
        services.AddScoped<
            IWebDriverCommandHandler<ClickOnElementCommand>,
            ClickOnElementCommandHandler
        >();
        services.AddScoped<IWebDriverCommandHandler<OpenPageCommand>, OpenPageCommandHandler>();
        services.AddScoped<
            IWebDriverCommandHandler<ScrollToDownCommand>,
            ScrollToDownCommandHandler
        >();
        services.AddScoped<
            IWebDriverCommandHandler<ScrollToTopCommand>,
            ScrollToTopCommandHandler
        >();
        services.AddScoped<
            IWebDriverCommandHandler<StartWebDriverCommand>,
            StartWebDriverCommandHandler
        >();
        services.AddScoped<
            IWebDriverCommandHandler<StopWebDriverCommand>,
            StopWebDriverCommandHandler
        >();
    }

    private static void RegisterQueries(this IServiceCollection services)
    {
        services.AddScoped<
            IWebDriverQueryHandler<GetElementQuery, WebElementObject>,
            GetElementQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetElementInsideOfElementQuery, WebElementObject>,
            GetElementInsideOfElementQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetElementsInsideOfElementQuery, WebElementObject[]>,
            GetElementsInsideOfElementQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetTextFromElementQuery, string>,
            GetTextElementFromQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetPageHtmlQuery, string>,
            GetPageHtmlQueryHandler
        >();
    }

    private static void RegisterValidators(this IServiceCollection services)
    {
        services.AddScoped<DriverStartDataDTOValidator>();
        services.AddScoped<ElementPathDataDTOValidator>();
        services.AddScoped<ExistingElementDTOValidator>();
        services.AddScoped<WebPageDataDTOValidator>();
    }

    private static void RegisterWebDriverDependencies(this IServiceCollection services)
    {
        services.AddSingleton<WebDriverInstance>();
        services.AddSingleton<WebDriverDispatcher>();
        services.AddSingleton<WebDriverApi>();
    }
}
