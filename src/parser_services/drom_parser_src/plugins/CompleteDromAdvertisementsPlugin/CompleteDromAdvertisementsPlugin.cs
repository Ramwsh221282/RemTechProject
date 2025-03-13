using AvitoParser.PDK.Models;
using CompleteDromAdvertisementsPlugin.Contexts;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CompleteDromAdvertisementsPlugin;

[Plugin(PluginName = nameof(CompleteDromAdvertisementsPlugin))]
public sealed class CompleteDromAdvertisementsPlugin : IPlugin<IEnumerable<ScrapedAdvertisement>>
{
    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CompleteDromAdvertisementsPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IEnumerable<ScrapedAdvertisement>> catalogueAdvertisementsUnwrap = resolver.Resolve<
            IEnumerable<ScrapedAdvertisement>
        >();
        if (catalogueAdvertisementsUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CompleteDromAdvertisementsPlugin),
                nameof(IEnumerable<ScrapedAdvertisement>)
            );
        IEnumerable<ScrapedAdvertisement> catalogueAdvertisements =
            catalogueAdvertisementsUnwrap.Value;

        Result<IBrowser> browserUnwrap = resolver.Resolve<IBrowser>();
        if (browserUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CompleteDromAdvertisementsPlugin),
                nameof(IBrowser)
            );
        IBrowser browser = browserUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CompleteDromAdvertisementsPlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        AdvertisemenetParsingContext scrapingContext = new AdvertisemenetParsingContext(
            browser,
            catalogueAdvertisements,
            logger,
            context
        );

        IEnumerable<ScrapedAdvertisement> results = await scrapingContext.Process();
        return Result<IEnumerable<ScrapedAdvertisement>>.Success(results);
    }
}
