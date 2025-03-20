using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.ElementBehavior;

public interface IElementBehavior<T>
{
    public Task<T> Invoke(IElementHandle element);
}
