using PuppeteerExtraSharp;
using PuppeteerExtraSharp.Plugins.ExtraStealth;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace InstantiatePuppeteerPlugin;

[Plugin(PluginName = nameof(InstantiatePuppeteerPlugin))]
public sealed class InstantiatePuppeteerPlugin : IPlugin<IBrowser>
{
    public async Task<Result<IBrowser>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerResolve = resolver.Resolve<ILogger>();
        if (loggerResolve.IsFailure)
            return new Error($"No dependency in payload ${nameof(ILogger)}");

        ILogger logger = loggerResolve.Value;

        Result<PuppeteerLaunchOptions> launchOptionsResolve =
            resolver.Resolve<PuppeteerLaunchOptions>();
        if (launchOptionsResolve.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                logger,
                nameof(InstantiatePuppeteerPlugin),
                nameof(PuppeteerLaunchOptions)
            );

        PuppeteerLaunchOptions launchOptionsParameter = launchOptionsResolve.Value;
        ViewPortOptions viewPortOptions = new ViewPortOptions()
        {
            Width = launchOptionsParameter.Width,
            Height = launchOptionsParameter.Height,
            DeviceScaleFactor = launchOptionsParameter.DeviceScale,
        };

        LaunchOptions launchOptions = new LaunchOptions()
        {
            Headless = launchOptionsParameter.Headless,
            Args = launchOptionsParameter.Arguments,
            DefaultViewport = viewPortOptions,
        };

        PuppeteerExtra extra = new PuppeteerExtra();
        StealthPlugin stealthPlugin = new StealthPlugin();
        extra = extra.Use(stealthPlugin);
        IBrowser browser = await extra.LaunchAsync(launchOptions);

        logger.Information(
            "{Context} instantiated puppeteer browser instance",
            nameof(InstantiatePuppeteerPlugin)
        );

        return Result<IBrowser>.Success(browser);
    }
}
