using PuppeteerSharp;
using SharedParsersLibrary.Puppeteer.Features.PageCreation;

namespace SharedParsersLibrary.Puppeteer.Features.BrowserCreation;

public static class IBrowserExtensions
{
    public static async Task<IPage> CreateByDomLoadStrategy(this IBrowser browser, string url)
    {
        IPageCreationStrategy strategy = new DomLoadPageCreationStrategy(browser);
        PageFactory factory = strategy.CreateFactoryByStrategy();
        return await factory.Create(url);
    }

    public static async Task<IPage> CreateByDomLoadNoImages(this IBrowser browser, string url)
    {
        IPageCreationStrategy strategy = new DomLoadPageCreationStrategyNoImages(browser);
        PageFactory factory = strategy.CreateFactoryByStrategy();
        return await factory.Create(url);
    }

    public static async Task<IPage> CreateByFullLoadstrategy(this IBrowser browser, string url)
    {
        IPageCreationStrategy strategy = new FullLoadPageCreationStrategy(browser);
        PageFactory factory = strategy.CreateFactoryByStrategy();
        return await factory.Create(url);
    }

    public static async Task<IPage> CreateByNetworkIdle0Strategy(this IBrowser browser, string url)
    {
        IPageCreationStrategy strategy = new NetWorkIdle0PageCreationStrategy(browser);
        PageFactory factory = strategy.CreateFactoryByStrategy();
        return await factory.Create(url);
    }

    public static PageFactory CreateFactoryByStrategy(this IPageCreationStrategy strategy) =>
        new PageFactory(strategy);

    public static async Task<IPage> CreateByNetworkIdle2Strategy(this IBrowser browser, string url)
    {
        IPageCreationStrategy strategy = new NetWorkIdle2PageCreationStrategy(browser);
        PageFactory factory = strategy.CreateFactoryByStrategy();
        return await factory.Create(url);
    }
}
