using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace InstantiateDromScrapingBrowserPlugin;

[Plugin(PluginName = nameof(InstantiateDromScrapingBrowserPlugin))]
public sealed class InstantiateDromScrapingBrowserPlugin : IPlugin<IBrowser>
{
    public async Task<Result<IBrowser>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);

        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(InstantiateDromScrapingBrowserPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<PuppeteerLaunchOptions> launchOptionsUnwrap =
            resolver.Resolve<PuppeteerLaunchOptions>();
        if (launchOptionsUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(InstantiateDromScrapingBrowserPlugin),
                nameof(PuppeteerLaunchOptions)
            );
        PuppeteerLaunchOptions launchOptions = launchOptionsUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(InstantiateDromScrapingBrowserPlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        Result installation = await context.Execute(
            "LoadPuppeteerPlugin",
            new PluginPayload(logger)
        );
        if (installation.IsFailure)
        {
            logger.Error(
                "{Context} can't install puppeteer browser",
                nameof(InstantiateDromScrapingBrowserPlugin)
            );
            return installation.Error;
        }

        Result<IBrowser> instantiation = await context.Execute<IBrowser>(
            "InstantiatePuppeteerPlugin",
            new PluginPayload(logger, launchOptions)
        );
        if (instantiation.IsFailure)
        {
            logger.Error(
                "{Context} can't start puppeteer browser",
                nameof(InstantiateDromScrapingBrowserPlugin)
            );
            return instantiation.Error;
        }

        IBrowser browser = instantiation.Value;
        return Result<IBrowser>.Success(browser);
    }
}
