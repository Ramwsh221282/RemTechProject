using System.Text;
using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreateAvitoAdvertisementLinksListPlugin;

[Plugin(PluginName = nameof(CreateAvitoAdvertisementLinksListPlugin))]
public sealed class CreateAvitoAdvertisementLinksListPlugin : IPlugin<IEnumerable<ScrapedSourceUrl>>
{
    private const string itemsContainerSelector = "items-items-pZX46";
    private const string itemSelector = "iva-item-root-Se7z4";
    private const string sliderClass = "iva-item-sliderLink-Fvfau";
    private const string href = "href";
    private const string avitoDomain = "https://www.avito.ru";

    public async Task<Result<IEnumerable<ScrapedSourceUrl>>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementLinksListPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementLinksListPlugin),
                nameof(IPlugin)
            );
        IPage page = pageUnwrap.Value;

        Result<PluginExecutionContext> contextUnwrap = resolver.Resolve<PluginExecutionContext>();
        if (contextUnwrap.IsFailure)
            logger.PluginDependencyMissingError(
                nameof(CreateAvitoAdvertisementLinksListPlugin),
                nameof(PluginExecutionContext)
            );
        PluginExecutionContext context = contextUnwrap.Value;

        QuerySelectorPayload querySelector = new QuerySelectorPayload(itemsContainerSelector);
        PluginCommand getItemsContainer = new PluginCommand(
            "GetElementPlugin",
            new PluginPayload(logger, page, querySelector)
        );

        Result<IElementHandle> containerResult = await context.ExecutePlugin<IElementHandle>(
            getItemsContainer
        );
        if (containerResult.IsFailure)
        {
            logger.Error(
                "{Context} cannot get container of advertisements in avito catalogue.",
                nameof(CreateAvitoAdvertisementLinksListPlugin)
            );
            return Result<IEnumerable<ScrapedSourceUrl>>.Success([]);
        }

        IElementHandle container = containerResult.Value;

        QuerySelectorPayload querySelectorForChildren = new QuerySelectorPayload(itemSelector);
        PluginCommand getChildren = new PluginCommand(
            "GetElementChildrenPlugin",
            new PluginPayload(logger, container, querySelectorForChildren)
        );

        Result<IEnumerable<IElementHandle>> childrenResult = await context.ExecutePlugin<
            IEnumerable<IElementHandle>
        >(getChildren);
        if (childrenResult.IsFailure)
        {
            logger.Error(
                "{Context} cannot get children of advertisements container",
                nameof(CreateAvitoAdvertisementLinksListPlugin)
            );
            return Result<IEnumerable<ScrapedSourceUrl>>.Success([]);
        }

        IElementHandle[] children = childrenResult.Value.ToArray();
        ScrapedSourceUrl[] urls = new ScrapedSourceUrl[children.Length];
        int lastIndex = 0;
        foreach (IElementHandle child in children)
        {
            QuerySelectorPayload sliderSelector = new QuerySelectorPayload(sliderClass);
            QueryAttributePayload hrefAttribute = new QueryAttributePayload(href);
            PluginPayload sliderPayload = new PluginPayload(logger, child, sliderSelector);
            PluginCommand getSlider = new PluginCommand("GetElementInElementPlugin", sliderPayload);

            Result<IElementHandle> sliderElementResult =
                await context.ExecutePlugin<IElementHandle>(getSlider);
            IElementHandle sliderElement = sliderElementResult.Value;

            PluginPayload attributePayload = new PluginPayload(
                logger,
                sliderElement,
                hrefAttribute
            );
            PluginCommand getHrefAttribute = new PluginCommand(
                "GetElementAttributePlugin",
                attributePayload
            );
            Result<string> attributeResult = await context.ExecutePlugin<string>(getHrefAttribute);

            string attribute = attributeResult.Value;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(avitoDomain);
            stringBuilder.Append(attribute);

            ScrapedSourceUrl urlResult = ScrapedSourceUrl.Create(stringBuilder.ToString());
            urls[lastIndex] = urlResult;
            lastIndex++;
            logger.Information("Advertisement Url: {Url}", urlResult.SourceUrl);
        }

        return Result<IEnumerable<ScrapedSourceUrl>>.Success(urls);
    }
}
