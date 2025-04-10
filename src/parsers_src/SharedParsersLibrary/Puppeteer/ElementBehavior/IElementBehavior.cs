using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.ElementBehavior;

public interface IElementBehavior<T>
{
    public Task<T> Invoke(IElementHandle element);
}
