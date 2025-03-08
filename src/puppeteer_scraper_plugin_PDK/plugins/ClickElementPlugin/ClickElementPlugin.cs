using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace ClickElementPlugin;

[Plugin(PluginName = nameof(ClickElementPlugin))]
public sealed class ClickElementPlugin : IPlugin
{
    public async Task<Result> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(ClickElementPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IElementHandle> elementUnwrap = resolver.Resolve<IElementHandle>();
        if (elementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(ClickElementPlugin),
                nameof(IElementHandle)
            );
        IElementHandle elementHandle = elementUnwrap.Value;

        try
        {
            await elementHandle.ClickAsync();
            logger.Information("{Context} element was clicked", nameof(ClickElementPlugin));
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} element was not clicked. Exception: {Ex}",
                nameof(ClickElementPlugin),
                ex.Message
            );
            return new Error("Element was not clicked. Inner plugin exception");
        }
    }
}
