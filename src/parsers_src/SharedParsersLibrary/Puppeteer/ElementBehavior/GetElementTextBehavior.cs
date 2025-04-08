using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.ElementBehavior;

public sealed class GetElementTextBehavior : IElementBehavior<string?>
{
    private const string script = "el => el.textContent";

    public async Task<string?> Invoke(IElementHandle element) =>
        await element.EvaluateFunctionAsync<string>(script);
}
