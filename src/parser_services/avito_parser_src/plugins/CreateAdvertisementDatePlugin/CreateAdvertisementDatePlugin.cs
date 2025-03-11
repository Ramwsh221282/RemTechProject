using AvitoParser.PDK.Models.ValueObjects;
using CreateAdvertisementDatePlugin.DateConverters;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAdvertisementDatePlugin;

[Plugin(PluginName = nameof(CreateAdvertisementDatePlugin))]
public sealed class CreateAdvertisementDatePlugin : IPlugin<ScrapedAdvertisementDate>
{
    private const string selectorClass = "span[data-marker='item-view/item-date']";

    public async Task<Result<ScrapedAdvertisementDate>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAdvertisementDatePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAdvertisementDatePlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        IElementHandle? dateElement = await page.QuerySelectorAsync(selectorClass);
        if (dateElement == null)
        {
            logger.Error("{Context} can't get date", nameof(CreateAdvertisementDatePlugin));
            return new Error("Can't get date");
        }

        string dateTextValue = await dateElement.EvaluateFunctionAsync<string>(
            "el => el.textContent"
        );
        logger.Information("Date string representation: {Date}", dateTextValue);

        CompositeDateConverter converter = new CompositeDateConverter(dateTextValue);
        Result<DateTime> dateResult = converter.ConvertToDateTime();
        if (dateResult.IsFailure)
            return dateResult.Error;

        return new ScrapedAdvertisementDate(dateResult.Value);
    }
}
