using PuppeteerSharp;

namespace SharedParsersLibrary.Puppeteer.PageCreation;

public interface IPageCreationStrategy
{
    public Task<IPage> Create(string pageUrl);
}
