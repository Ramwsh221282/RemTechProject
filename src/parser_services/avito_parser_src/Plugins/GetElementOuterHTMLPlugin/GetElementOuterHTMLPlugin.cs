using AvitoParser.PDK;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementOuterHTMLPlugin;

[Plugin(PluginName = nameof(GetElementOuterHTMLPlugin))]
public sealed class GetElementOuterHTMLPlugin : IAvitoPlugin<string>
{
    private const string script = "el => el.outerHTML";

    public async Task<Result<string>> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementOuterHTMLPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IElementHandle> elementUnwrap = resolver.Resolve<IElementHandle>();
        if (elementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementOuterHTMLPlugin),
                nameof(IElementHandle)
            );
        IElementHandle element = elementUnwrap.Value;
        try
        {
            string? outerHtml = await element.EvaluateFunctionAsync<string>(script);
            return string.IsNullOrWhiteSpace(outerHtml)
                ? new Error("Cannot get outer html of element")
                : outerHtml;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} cannot get outer html of element. Exception: {Exception}",
                nameof(GetElementOuterHTMLPlugin),
                ex.Message
            );
            return new Error(
                $"{nameof(GetElementOuterHTMLPlugin)} cannot get outer html of element."
            );
        }
    }
}
