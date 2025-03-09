using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAdvertisementDescriptionPlugin;

[Plugin(PluginName = nameof(CreateAdvertisementDescriptionPlugin))]
public sealed class CreateAdvertisementDescriptionPlugin : IPlugin<ScrapedDescription>
{
    private const string descriptionSelectorAttribute =
        "div[data-marker='item-view/item-description']";

    private const string script = "el => el.textContent";

    public async Task<Result<ScrapedDescription>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAdvertisementDescriptionPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            logger.PluginDependencyMissingError(
                nameof(CreateAdvertisementDescriptionPlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        IElementHandle? descriptionElement = await page.QuerySelectorAsync(
            descriptionSelectorAttribute
        );
        if (descriptionElement == null)
        {
            logger.Error(
                "{Context} unable to find description",
                nameof(CreateAdvertisementDescriptionPlugin)
            );
            return new Error("Unable to find description");
        }

        string description = await descriptionElement.EvaluateFunctionAsync<string>(script);
        return ScrapedDescription.Create(description);
    }
}
