using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementChildrenPlugin;

[Plugin(PluginName = nameof(GetElementChildrenPlugin))]
public sealed class GetElementChildrenPlugin : IPlugin<IEnumerable<IElementHandle>>
{
    public async Task<Result<IEnumerable<IElementHandle>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementChildrenPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IElementHandle> elementUnwrap = resolver.Resolve<IElementHandle>();
        if (elementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementChildrenPlugin),
                nameof(IElementHandle)
            );
        IElementHandle elementHandle = elementUnwrap.Value;

        Result<QuerySelectorPayload> querySelectorUnwrap = resolver.Resolve<QuerySelectorPayload>();
        if (querySelectorUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementChildrenPlugin),
                nameof(QuerySelectorPayload)
            );
        QuerySelectorPayload querySelector = querySelectorUnwrap.Value;

        try
        {
            IElementHandle[]? elements = await elementHandle.QuerySelectorAllAsync(
                querySelector.Selector
            );
            return elements ?? Result<IEnumerable<IElementHandle>>.Success([]);
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context}. Failed to get children of element. Exception: {Exception}",
                nameof(GetElementChildrenPlugin),
                ex.Message
            );
            return new Error("Failed to get childre of elements. Inner plugin exception");
        }
    }
}
