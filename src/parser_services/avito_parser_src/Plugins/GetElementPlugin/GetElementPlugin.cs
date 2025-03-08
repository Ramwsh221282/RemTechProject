using AvitoParser.PDK;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementPlugin;

[Plugin(PluginName = "GetElementPlugin")]
public sealed class GetElementPlugin : IAvitoPlugin<IElementHandle>
{
    public async Task<Result<IElementHandle>> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(nameof(GetElementPlugin), nameof(IPage));
        IPage page = pageUnwrap.Value;

        Result<QuerySelectorPayload> querySelectorUnwrap = resolver.Resolve<QuerySelectorPayload>();
        if (querySelectorUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(nameof(GetElementPlugin), nameof(IPage));
        QuerySelectorPayload querySelector = querySelectorUnwrap.Value;

        try
        {
            IElementHandle? element = await page.QuerySelectorAsync(querySelector.Selector);
            return element == null
                ? new Error($"Element with query selector {querySelector.Selector} not found")
                : Result<IElementHandle>.Success(element);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} failed. Exception: {Message}",
                nameof(GetElementPlugin),
                ex.Message
            );
            return new Error(
                $"Element with query selector {querySelector.Selector} internal plugin error"
            );
        }
    }
}
