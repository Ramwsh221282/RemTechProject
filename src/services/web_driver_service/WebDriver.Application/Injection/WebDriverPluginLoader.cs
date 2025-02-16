using Microsoft.Extensions.DependencyInjection;
using WebDriver.Application.Commands.ClearPool;
using WebDriver.Application.Commands.ClearText;
using WebDriver.Application.Commands.ClickOnElement;
using WebDriver.Application.Commands.OpenPage;
using WebDriver.Application.Commands.ScrollElement;
using WebDriver.Application.Commands.ScrollToDown;
using WebDriver.Application.Commands.ScrollToTop;
using WebDriver.Application.Commands.SendTextOnElement;
using WebDriver.Application.Commands.SendTextOnElementWithoutClear;
using WebDriver.Application.Commands.StartWebDriver;
using WebDriver.Application.Commands.StopWebDriver;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Application.Queries.GetElement;
using WebDriver.Application.Queries.GetElementAttribute;
using WebDriver.Application.Queries.GetElementHtml;
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
        services.AddScoped<
            IWebDriverCommandHandler<SendTextOnElementCommand>,
            SendTextOnElementCommandHandler
        >();
        services.AddScoped<
            IWebDriverCommandHandler<ScrollElementCommand>,
            ScrollElementCommandHandler
        >();
        services.AddScoped<
            IWebDriverCommandHandler<SendTextOnElementWithoutClearCommand>,
            SendTextOnElementWithoutClearCommandHandler
        >();
        services.AddScoped<IWebDriverCommandHandler<ClearTextCommand>, ClearTextCommandHandler>();
        services.AddScoped<IWebDriverCommandHandler<ClearPoolCommand>, ClearPoolCommandHandler>();
    }

    private static void RegisterQueries(this IServiceCollection services)
    {
        services.AddScoped<
            IWebDriverQueryHandler<GetElementQuery, WebElementResponseObject>,
            GetElementQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetElementInsideOfElementQuery, WebElementResponseObject>,
            GetElementInsideOfElementQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetElementsInsideOfElementQuery, WebElementResponseObject[]>,
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
        services.AddScoped<
            IWebDriverQueryHandler<GetElementHtmlQuery, string>,
            GetElementHtmlQueryHandler
        >();
        services.AddScoped<
            IWebDriverQueryHandler<GetElementAttributeQuery, string>,
            GetElementAttributeValueQueryHandler
        >();
    }

    private static void RegisterValidators(this IServiceCollection services)
    {
        services.AddSingleton<DriverStartDataDTOValidator>();
        services.AddSingleton<ElementPathDataDTOValidator>();
        services.AddSingleton<ExistingElementDTOValidator>();
        services.AddSingleton<WebPageDataDTOValidator>();
        services.AddScoped<ElementAttributeDTOValidator>();
    }

    private static void RegisterWebDriverDependencies(this IServiceCollection services)
    {
        services.AddSingleton<WebDriverInstance>();
        services.AddSingleton<WebDriverDispatcher>();
        services.AddSingleton<WebDriverApi>();
    }
}
