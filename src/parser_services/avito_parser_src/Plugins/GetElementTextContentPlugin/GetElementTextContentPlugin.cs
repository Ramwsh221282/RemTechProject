using AvitoParser.PDK;
using PuppeteerSharp;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace GetElementTextContentPlugin;

[Plugin(PluginName = nameof(GetElementTextContentPlugin))]
public sealed class GetElementTextContentPlugin : IAvitoPlugin<string>
{
    private const string script = "el => el.textContent";

    public async Task<Result<string>> Execute(AvitoPluginPayload? payload)
    {
        AvitoPluginPayloadResolver resolver = new AvitoPluginPayloadResolver(payload);
        Result<ILogger> loggerUnwrap = resolver.Resolve<ILogger>();
        if (loggerUnwrap.IsFailure)
            return PluginPDKErrors.PluginDependencyMissingError(
                nameof(GetElementTextContentPlugin),
                nameof(ILogger)
            );
        ILogger logger = loggerUnwrap.Value;

        Result<IElementHandle> elementUnwrap = resolver.Resolve<IElementHandle>();
        if (elementUnwrap.IsFailure)
            return logger.PluginDependencyMissingError(
                nameof(GetElementTextContentPlugin),
                nameof(IElementHandle)
            );
        IElementHandle element = elementUnwrap.Value;

        try
        {
            string? content = await element.EvaluateFunctionAsync<string>(script);
            return string.IsNullOrWhiteSpace(content)
                ? new Error("Can't get text content of element")
                : content;
        }
        catch (Exception ex)
        {
            logger.Fatal(
                "{Context} failed to get text content. Exception: {Exception}",
                nameof(GetElementTextContentPlugin),
                ex.Message
            );
            return new Error("Can't get text content of element. Inner plugin exception");
        }
    }
}
