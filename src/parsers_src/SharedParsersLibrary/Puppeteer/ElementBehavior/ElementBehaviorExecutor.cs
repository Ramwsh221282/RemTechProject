using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.ElementBehavior;

public sealed class ElementBehaviorExecutor
{
    private IElementHandle _element;

    public ElementBehaviorExecutor(IElementHandle element)
    {
        _element = element;
    }

    public async Task<T> Invoke<T>(IElementBehavior<T> behavior)
    {
        return await behavior.Invoke(_element);
    }

    public void SwapElement(IElementHandle element) => _element = element;
}
