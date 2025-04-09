using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.ElementBehavior;

public sealed class GetChildBehavior : IElementBehavior<IElementHandle?>
{
    private readonly string _selector;

    public GetChildBehavior(string selector)
    {
        _selector = selector;
    }

    public async Task<IElementHandle?> Invoke(IElementHandle element)
    {
        return await element.QuerySelectorAsync(_selector);
    }
}
