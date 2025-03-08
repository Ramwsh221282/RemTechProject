using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementInElementPlugin;

[Plugin(PluginName = nameof(GetElementInElementPlugin))]
public sealed class GetElementInElementPlugin : IPlugin<IElementHandle>
{
    public async Task<Result<IElementHandle>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementInElementPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IElementHandle> parentElementUnwrap = resolver.Resolve<IElementHandle>();
        if (parentElementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementInElementPlugin),
                nameof(IElementHandle)
            );
        IElementHandle parent = parentElementUnwrap.Value;

        Result<QuerySelectorPayload> selectorUnwrap = resolver.Resolve<QuerySelectorPayload>();
        if (selectorUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementInElementPlugin),
                nameof(QuerySelectorPayload)
            );
        QuerySelectorPayload selector = selectorUnwrap.Value;

        try
        {
            IElementHandle? element = await parent.QuerySelectorAsync(selector.Selector);
            if (element == null)
            {
                logger.Error(
                    "{Context} cannot find child element in parent.",
                    nameof(GetElementInElementPlugin)
                );
                return new Error("Element not found");
            }

            return Result<IElementHandle>.Success(element);
        }
        catch (Exception ex)
        {
            logger.Error(
                "{Context} cannot find child element in parent. Exception: {Ex}",
                nameof(GetElementInElementPlugin),
                ex.Message
            );
            return new Error("Element not found. Inner plugin exception");
        }
    }
}
