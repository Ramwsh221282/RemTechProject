using AvitoParser.PDK;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace ScrollBottomPlugin;

[Plugin(PluginName = nameof(ScrollBottomPlugin))]
public sealed class ScrollBottomPlugin : IAvitoPlugin
{
    private const string getCurrentHeightScript = "document.body.scrollHeight";
    private const string scrollBottomScript = "window.scrollTo(0, document.body.scrollHeight)";

    public async Task<Result> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(ScrollBottomPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(nameof(ScrollBottomPlugin), nameof(IPage));

        IPage page = pageUnwrap.Value;

        int currentHeight = await CalculateCurrentDocumentHeight(page);
        while (true)
        {
            await ProcessScrollBottom(page, 2);
            int newHeight = await CalculateCurrentDocumentHeight(page);

            if (newHeight == currentHeight)
                break;

            currentHeight = newHeight;
        }

        logger.Information("{Context}. Page scrolled bottom", nameof(ScrollBottomPlugin));
        return Result.Success();
    }

    private async Task<int> CalculateCurrentDocumentHeight(IPage page) =>
        await page.EvaluateExpressionAsync<int>(getCurrentHeightScript);

    private async Task ProcessScrollBottom(IPage page, int delaySeconds)
    {
        await page.EvaluateExpressionAsync(scrollBottomScript);
        await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
    }
}
