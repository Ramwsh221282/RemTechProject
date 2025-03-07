using AvitoParser.PDK;
using PuppeteerSharp;
using PuppeteerSharp.BrowserData;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace LoadPuppeteerPlugin;

[Plugin(PluginName = nameof(LoadPuppeteerPlugin))]
public class LoadPuppeteerPlugin : IAvitoPlugin
{
    private const string BrowserCataloguePath = "Chrome";

    public async Task<Result> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerResult = resolver.Resolve<ILogger>();
        if (loggerResult.IsFailure)
            return loggerResult.Error;

        ILogger logger = loggerResult.Value;

        if (Directory.Exists(BrowserCataloguePath))
        {
            logger.Information("Browser is already installed");
            return Result.Success();
        }

        using IBrowserFetcher fetcher = new BrowserFetcher();
        try
        {
            InstalledBrowser installedBrowser = await fetcher.DownloadAsync();
            string executablePath = installedBrowser.GetExecutablePath();
            if (File.Exists(executablePath))
            {
                logger.Information(
                    "{Context} loaded plugin with path: {Path}",
                    nameof(LoadPuppeteerPlugin),
                    executablePath
                );
                return Result.Success();
            }

            logger.Error(
                "{Context} unable to determine executable path {Path}",
                nameof(LoadPuppeteerPlugin),
                executablePath
            );
            return new Error($"Unable to determine executable path {executablePath}");
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} failed at loading plugin. Exception: {Exception}",
                nameof(LoadPuppeteerPlugin),
                ex.Message
            );
            return new Error($"Unable to load plugin. Exception: {ex.Message}");
        }
    }
}
