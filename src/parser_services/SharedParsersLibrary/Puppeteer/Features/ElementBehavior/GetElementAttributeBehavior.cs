using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.ElementBehavior;

public sealed class GetElementAttributeBehavior : IElementBehavior<string?>
{
    private readonly string _attributeName;

    public GetElementAttributeBehavior(string attributeName) => _attributeName = attributeName;

    public async Task<string?> Invoke(IElementHandle element) =>
        await element.EvaluateFunctionAsync<string>($"el => el.getAttribute('{_attributeName}')");
}
