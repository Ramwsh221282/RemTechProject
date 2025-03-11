using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace OpenAdvertisementPagePlugin;

[Plugin(PluginName = nameof(OpenAdvertisementPagePlugin))]
public sealed class OpenAdvertisementPagePlugin : IPlugin<IPage>
{
    private const string script =
        @"() => {    
    Object.defineProperty(navigator, 'webdriver', { get: () => false });
    
    window.chrome = { runtime: {} };
    
    Object.defineProperty(navigator, 'languages', {get: () => ['ru-RU', 'ru']});
    Object.defineProperty(navigator, 'plugins', {
        get: () => [1, 2, 3, 4, 5],
    });
    
    const originalQuery = window.navigator.permissions.query;
    window.navigator.permissions.query = (parameters) => (
        parameters.name === 'notifications' ?
        Promise.resolve({ state: Notification.permission }) :
        originalQuery(parameters)
    );
    
    const getParameter = WebGLRenderingContext.prototype.getParameter;
    WebGLRenderingContext.prototype.getParameter = function(parameter) {
        if (parameter === 37445) {
            return 'Intel Inc.';
        }
        if (parameter === 37446) {
            return 'Intel Iris OpenGL Engine';
        }
        return getParameter(parameter);
    };
}";

    public async Task<Result<IPage>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(OpenAdvertisementPagePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IBrowser> browserUnwrap = resolver.Resolve<IBrowser>();
        if (browserUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(OpenAdvertisementPagePlugin),
                nameof(IBrowser)
            );
        IBrowser browser = browserUnwrap.Value;

        Result<ScrapedSourceUrl> urlUnwrap = resolver.Resolve<ScrapedSourceUrl>();
        if (urlUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(OpenAdvertisementPagePlugin),
                nameof(ScrapedSourceUrl)
            );
        string url = urlUnwrap.Value.SourceUrl;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(OpenAdvertisementPagePlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        try
        {
            IPage page = await browser.NewPageAsync();
            await page.EvaluateFunctionOnNewDocumentAsync(script);
            await page.GoToAsync(
                url,
                new NavigationOptions()
                {
                    WaitUntil = [WaitUntilNavigation.DOMContentLoaded],
                    Timeout = 300000,
                }
            );
            Console.WriteLine("Loaded");
            await context.Execute("ScrollBottomPlugin", new PluginPayload(logger, page));
            await context.Execute("ScrollTopPlugin", new PluginPayload(logger, page));
            return Result<IPage>.Success(page);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} failed. Exception: {Ex}",
                nameof(OpenAdvertisementPagePlugin),
                ex.Message
            );
            return new Error("Failed to open advertisement page. Plugin inner exception");
        }
    }
}
