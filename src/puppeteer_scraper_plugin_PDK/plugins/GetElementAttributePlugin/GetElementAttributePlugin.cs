using PuppeteerSharp;
using RemTech.Common.Plugin.PDK;
using RemTech.Puppeteer.Scraper.Plugin.PDK;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementAttributePlugin;

[Plugin(PluginName = nameof(GetElementAttributePlugin))]
public sealed class GetElementAttributePlugin : IPlugin<string>
{
    public async Task<Result<string>> Execute(PluginPayload? payload)
    {
        PluginPayloadResolver resolver = new PluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementAttributePlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<QueryAttributePayload> queryAttributeUnwrap =
            resolver.Resolve<QueryAttributePayload>();
        if (queryAttributeUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementAttributePlugin),
                nameof(QueryAttributePayload)
            );
        QueryAttributePayload queryAttribute = queryAttributeUnwrap.Value;

        Result<IElementHandle> elementUnwrap = resolver.Resolve<IElementHandle>();
        if (elementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementAttributePlugin),
                nameof(IElementHandle)
            );
        IElementHandle element = elementUnwrap.Value;
        try
        {
            string script = CreateGetAttributeScript(queryAttribute);
            string? attributeValue = await element.EvaluateFunctionAsync<string>(script);
            return string.IsNullOrWhiteSpace(attributeValue)
                ? new Error(
                    $"{nameof(GetElementAttributePlugin)} can't find attribute with name: {queryAttribute.AttributeName}"
                )
                : attributeValue;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} can't find attribute with name: {Name}. Exception: {Ex}",
                nameof(GetElementAttributePlugin),
                queryAttribute.AttributeName,
                ex.Message
            );
            return new Error(
                $"{nameof(GetElementAttributePlugin)} can't find attribute with name: {queryAttribute.AttributeName}"
            );
        }
    }

    private string CreateGetAttributeScript(QueryAttributePayload payload) =>
        $"el => el.getAttribute('{payload.AttributeName}')";
}
