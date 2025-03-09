using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAdvertisementTitlePlugin;

[Plugin(PluginName = nameof(CreateAdvertisementTitlePlugin))]
public sealed class CreateAdvertisementTitlePlugin : IPlugin<ScrapedTitle>
{
    private const string script = "el => el.textContent";

    public async Task<Result<ScrapedTitle>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAdvertisementTitlePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAdvertisementTitlePlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        try
        {
            IElementHandle element = await page.QuerySelectorAsync(
                "h1[data-marker='item-view/title-info']"
            );
            if (element == null)
            {
                logger.Error(
                    "{Context}. Cannot find title.",
                    nameof(CreateAdvertisementTitlePlugin)
                );
                return new Error("Cannot find title.");
            }

            string? text = await element.EvaluateFunctionAsync<string>(script);
            if (string.IsNullOrWhiteSpace(text))
            {
                logger.Error(
                    "{Context}. Cannot find title.",
                    nameof(CreateAdvertisementTitlePlugin)
                );
                return new Error("Cannot find title.");
            }

            await element.DisposeAsync();
            Result<ScrapedTitle> titleResult = ScrapedTitle.Create(text);
            if (!titleResult.IsFailure)
                return titleResult.Value;

            logger.Error(
                "{Context}. {Error}",
                nameof(CreateAdvertisementTitlePlugin),
                titleResult.Error.Description
            );
            return titleResult.Error;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} failed. Exception: {Ex}",
                nameof(CreateAdvertisementTitlePlugin),
                ex.Message
            );
            return new Error("Failed to create title. Inner plugin exception");
        }
    }
}
