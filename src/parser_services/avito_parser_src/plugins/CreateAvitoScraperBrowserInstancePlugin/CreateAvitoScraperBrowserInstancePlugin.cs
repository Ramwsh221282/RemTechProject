using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAvitoScraperBrowserInstancePlugin;

[Plugin(PluginName = nameof(CreateAvitoScraperBrowserInstancePlugin))]
public sealed class CreateAvitoScraperBrowserInstancePlugin : IPlugin<IBrowser>
{
    public async Task<Result<IBrowser>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAvitoScraperBrowserInstancePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoScraperBrowserInstancePlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        PuppeteerLaunchOptions options = new PuppeteerLaunchOptions(
            Arguments:
            [
                "--start-maximized",
                "--no-sandbox",
                "--disable-infobars",
                "--disable-dev-shm-usage",
                "--disable-web-security",
                "--disable-extensions",
                "--disable-setuid-sandbox",
                "--ignore-certificate-errors",
                "--disable-blink-features=AutomationControlled",
            ],
            Headless: false
        );

        PluginCommand instantiateCommand = new PluginCommand(
            "InstantiatePuppeteerPlugin",
            new PluginPayload(options, logger)
        );

        return await context.ExecutePlugin<IBrowser>(instantiateCommand);
    }
}
