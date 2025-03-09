using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAdvertisementAddressPlugin;

[Plugin(PluginName = nameof(CreateAdvertisementAddressPlugin))]
public sealed class CreateAdvertisementAddressPlugin : IPlugin<ScrapedAddress>
{
    private const string addressSelector = "style-item-address__string-wt61A";
    private const string script = "el => el.textContent";

    public async Task<Result<ScrapedAddress>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAdvertisementAddressPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAdvertisementAddressPlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        QuerySelectorPayload addressPayload = new QuerySelectorPayload(addressSelector);
        IElementHandle? addressElement = await page.QuerySelectorAsync(addressPayload.Selector);
        if (addressElement == null)
        {
            logger.Error(
                "{Context} can't find address string",
                nameof(CreateAdvertisementAddressPlugin)
            );
            return new Error("Can't find address string");
        }

        string addressValue = await addressElement.EvaluateFunctionAsync<string>(script);
        return ScrapedAddress.Create(addressValue);
    }
}
