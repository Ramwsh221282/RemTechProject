using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAvitoAdvertisementPricePlugin;

[Plugin(PluginName = nameof(CreateAvitoAdvertisementPricePlugin))]
public sealed class CreateAvitoAdvertisementPricePlugin : IPlugin<ScrapedPrice>
{
    private const string priceContainerSelector = "style-price-value-main-TIg6u";
    private const string priceAttribute = "content";
    private const string priceAdditionalSelector = "style-price-value-additional-pFInr";

    public async Task<Result<ScrapedPrice>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementPricePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementPricePlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementPricePlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        QuerySelectorPayload priceSelector = new QuerySelectorPayload(priceContainerSelector);
        IElementHandle? priceElement = await page.QuerySelectorAsync(priceSelector.Selector);
        if (priceElement == null)
        {
            logger.Error(
                "{Context} can't find price element",
                nameof(CreateAvitoAdvertisementPricePlugin)
            );
            return new Error("Can't find price element");
        }

        IElementHandle? priceNode = await priceElement.QuerySelectorAsync("span[itemprop='price']");
        if (priceNode == null)
        {
            logger.Error(
                "{Context} can't find price element",
                nameof(CreateAvitoAdvertisementPricePlugin)
            );
            return new Error("Can't find price element");
        }

        QueryAttributePayload attributePayload = new QueryAttributePayload(priceAttribute);
        PluginCommand getAttribute = new PluginCommand(
            "GetElementAttributePlugin",
            new PluginPayload(priceNode, attributePayload, logger)
        );
        Result<string> attributeResult = await context.ExecutePlugin<string>(getAttribute);
        if (attributeResult.IsFailure)
        {
            logger.Error(
                "{Context} can't get price value attribute",
                nameof(CreateAvitoAdvertisementPricePlugin)
            );
            return new Error("Can't get price value attribute");
        }

        string priceValueString = attributeResult.Value;

        QuerySelectorPayload additionalSelector = new QuerySelectorPayload(priceAdditionalSelector);
        IElementHandle? additionalElement = await page.QuerySelectorAsync(
            additionalSelector.Selector
        );
        string additionalPriceDescription = "Нет";
        if (additionalElement != null)
        {
            PluginCommand getElementText = new PluginCommand(
                "GetElementTextContentPlugin",
                new PluginPayload(logger, additionalElement)
            );
            Result<string> textResult = await context.ExecutePlugin<string>(getElementText);
            if (textResult.IsSuccess)
                additionalPriceDescription = textResult.Value;
        }

        Result<ScrapedPrice> priceResult = ScrapedPrice.Create(
            priceValueString,
            additionalPriceDescription
        );

        if (!priceResult.IsFailure)
            return priceResult.Value;

        logger.Error(
            "{Context} {Error}",
            nameof(CreateAvitoAdvertisementPricePlugin),
            priceResult.Error.Description
        );
        return priceResult.Error;
    }
}
