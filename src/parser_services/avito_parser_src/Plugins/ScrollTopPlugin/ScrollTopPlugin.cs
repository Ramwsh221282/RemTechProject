using AvitoParser.PDK;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace ScrollTopPlugin;

[Plugin(PluginName = nameof(ScrollTopPlugin))]
public sealed class ScrollTopPlugin : IAvitoPlugin
{
    private const string calculateCurrentHeightScript = "document.body.scrollHeight";
    private const string scrollTopScript = "window.scrollTo(0, 0)";

    public async Task<Result> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(ScrollTopPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(nameof(ScrollTopPlugin), nameof(IPage));

        IPage page = pageUnwrap.Value;
        int currentHeight = await CalculateCurrentHeight(page);
        while (true)
        {
            await PerformScrollTop(page, 2);

            int newHeight = await CalculateCurrentHeight(page);

            if (newHeight == currentHeight)
                break;

            currentHeight = newHeight;
        }

        logger.Information("{Context} scrolled top", nameof(ScrollTopPlugin));
        return Result.Success();
    }

    private async Task<int> CalculateCurrentHeight(IPage page) =>
        await page.EvaluateExpressionAsync<int>(calculateCurrentHeightScript);

    private async Task PerformScrollTop(IPage page, int delaySeconds)
    {
        await page.EvaluateExpressionAsync(scrollTopScript);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
    }
}
