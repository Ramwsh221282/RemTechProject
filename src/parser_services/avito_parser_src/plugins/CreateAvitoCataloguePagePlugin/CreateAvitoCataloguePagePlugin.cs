using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAvitoCataloguePagePlugin;

[Plugin(PluginName = nameof(CreateAvitoCataloguePagePlugin))]
public sealed class CreateAvitoCataloguePagePlugin : IPlugin<IPage>
{
    public async Task<Result<IPage>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAvitoCataloguePagePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IBrowser> browserUnwrap = resolver.Resolve<IBrowser>();
        if (browserUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoCataloguePagePlugin),
                nameof(IBrowser)
            );
        IBrowser browser = browserUnwrap.Value;

        Result<ScrapedSourceUrl> urlUnwrap = resolver.Resolve<ScrapedSourceUrl>();
        if (urlUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoCataloguePagePlugin),
                nameof(ScrapedSourceUrl)
            );
        ScrapedSourceUrl url = urlUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoCataloguePagePlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        IPage page = await browser.NewPageAsync();
        await page.GoToAsync(
            url.SourceUrl,
            new NavigationOptions { WaitUntil = [WaitUntilNavigation.DOMContentLoaded] }
        );

        PluginCommand scrollBottomCommand = new PluginCommand(
            "ScrollBottomPlugin",
            new PluginPayload(logger, page)
        );
        Result scrollingResult = await context.ExecutePlugin(scrollBottomCommand);
        if (scrollingResult.IsFailure)
            return scrollingResult.Error;

        PluginCommand scrollTopCommand = new PluginCommand(
            "ScrollTopPlugin",
            new PluginPayload(logger, page)
        );

        Result scrollingTopResult = await context.ExecutePlugin(scrollTopCommand);
        return scrollingTopResult.IsFailure
            ? scrollingTopResult.Error
            : Result<IPage>.Success(page);
    }
}
