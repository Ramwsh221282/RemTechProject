using AvitoParser.PDK.Models;
using AvitoParser.PDK.Models.ValueObjects;
using CollectAdvertisementsDataFromCataloguePlugin.Contexts;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CollectAdvertisementsDataFromCataloguePlugin;

[Plugin(PluginName = nameof(CollectAdvertisementsDataFromCataloguePlugin))]
public sealed class CollectAdvertisementsDataFromCataloguePlugin
    : IPlugin<IEnumerable<ScrapedAdvertisement>>
{
    public async Task<Result<IEnumerable<ScrapedAdvertisement>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CollectAdvertisementsDataFromCataloguePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IBrowser> browserUnwrap = resolver.Resolve<IBrowser>();
        if (browserUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CollectAdvertisementsDataFromCataloguePlugin),
                nameof(IBrowser)
            );
        IBrowser browser = browserUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CollectAdvertisementsDataFromCataloguePlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        Result<ScrapedSourceUrl> inputUrlUnwrap = resolver.Resolve<ScrapedSourceUrl>();
        if (inputUrlUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CollectAdvertisementsDataFromCataloguePlugin),
                nameof(ScrapedSourceUrl)
            );
        ScrapedSourceUrl inputUrl = inputUrlUnwrap.Value;

        DromCatalogueScrapingContext catalogueScraping = new DromCatalogueScrapingContext(
            logger,
            browser,
            context,
            inputUrl
        );

        return await catalogueScraping.Process();
    }
}
