using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public interface IPageBehavior
{
    Task Invoke(IPage page);
}

public interface IPageBehavior<T>
{
    Task<T> Invoke(IPage page);
}
