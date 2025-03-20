using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageCreation;

public sealed class PageFactory
{
    private readonly IPageCreationStrategy _strategy;

    public PageFactory(IPageCreationStrategy strategy)
    {
        _strategy = strategy;
    }

    public async Task<IPage> Create(string sourceUrl)
    {
        return await _strategy.Create(sourceUrl);
    }
}
