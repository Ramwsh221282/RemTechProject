using AvitoParser.PDK.Models.ValueObjects;
using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace CreatePublisherPlugin;

[Plugin(PluginName = nameof(CreatePublisherPlugin))]
public sealed class CreatePublisherPlugin : IPlugin<ScrapedPublisher>
{
    private const string publisherClass = "div[data-marker='seller-info/name']";

    public async Task<Result<ScrapedPublisher>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(CreatePublisherPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IPage> pageUnwrap = resolver.Resolve<IPage>();
        if (pageUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(CreatePublisherPlugin),
                nameof(IPage)
            );
        IPage page = pageUnwrap.Value;

        IElementHandle? publisherElement = await page.QuerySelectorAsync(publisherClass);
        if (publisherElement == null)
        {
            logger.Error(
                "{Context} can't get publisher information",
                nameof(CreatePublisherPlugin)
            );
            return new Error("Can't get publisher information");
        }

        string text = await publisherElement.EvaluateFunctionAsync<string>("el => el.textContent");
        return ScrapedPublisher.Create(text);
    }
}
