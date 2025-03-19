using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.Features.PageCreation;

public interface IPageCreationStrategy
{
    public Task<IPage> Create(string pageUrl);
}
