using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public class PageBehaviorExecutor
{
    private IPage _page;

    public PageBehaviorExecutor(IPage page) => _page = page;

    public async Task<T> Invoke<T>(IPageBehavior<T> behavior) => await behavior.Invoke(_page);

    public async Task Invoke(IPageBehavior behavior) => await behavior.Invoke(_page);

    public void SwapPage(IPage page) => _page = page;
}
