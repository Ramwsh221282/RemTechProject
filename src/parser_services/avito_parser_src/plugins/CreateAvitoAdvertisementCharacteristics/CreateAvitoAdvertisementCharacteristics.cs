using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAvitoAdvertisementCharacteristics;

[Plugin(PluginName = nameof(CreateAvitoAdvertisementCharacteristics))]
public sealed class CreateAvitoAdvertisementCharacteristics
    : IPlugin<IReadOnlyCollection<ScrapedCharacteristic>>
{
    private const string parametersListClass = "params-paramsList-_awNW";
    private const string characteristicClass = "params-paramsList__item-_2Y2O";
    private const string script = "el => el.textContent";

    public async Task<Result<IReadOnlyCollection<ScrapedCharacteristic>>> Execute(
        PluginPayload? payload
    )
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementCharacteristics),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementCharacteristics),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        QuerySelectorPayload parametersListPayload = new QuerySelectorPayload(parametersListClass);
        IElementHandle? elementHandle = await page.QuerySelectorAsync(
            parametersListPayload.Selector
        );
        if (elementHandle == null)
        {
            logger.Error(
                "{Context} failed to get parameters list",
                nameof(CreateAvitoAdvertisementCharacteristics)
            );
            return new Error("Failed to get parameters list. Not found.");
        }

        QuerySelectorPayload characteristicPayload = new QuerySelectorPayload(characteristicClass);
        IElementHandle[] characteristics = await elementHandle.QuerySelectorAllAsync(
            characteristicPayload.Selector
        );
        List<ScrapedCharacteristic> scrapedCharacteristics = [];
        foreach (var element in characteristics)
        {
            string ctx = await element.EvaluateFunctionAsync<string>(script);
            if (!string.IsNullOrWhiteSpace(ctx))
            {
                string[] splitted = ctx.Split(':');
                string name = splitted[0];
                string value = splitted[1];
                value = value.Replace("?", "").Replace("*", "");
                Result<ScrapedCharacteristic> characteristic = ScrapedCharacteristic.Create(
                    name,
                    value
                );
                scrapedCharacteristics.Add(characteristic);
            }
        }

        return scrapedCharacteristics;
    }
}
