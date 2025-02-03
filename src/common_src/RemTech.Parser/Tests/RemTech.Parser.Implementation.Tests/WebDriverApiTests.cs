using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Injection;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Tests;

public sealed class WebDriverApiTests
{
    private readonly IWebDriverApi _api;

    public WebDriverApiTests()
    {
        IServiceCollection services = new ServiceCollection();
        services = services.AddCommonInjections();
        services = new WebDriverParserPluginInjection().Inject(services);
        IServiceProvider provider = services.BuildServiceProvider();
        _api = provider.GetRequiredService<IWebDriverApi>();
    }

    [Fact]
    public async Task Open_WebDriver_Page_Test()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }

    [Fact]
    public async Task Open_WebDriver_Page_And_Scroll_Down_And_Bottom_Test()
    {
        Result starting = await _api.ExecuteCommand(new StartWebDriverCommand());
        Assert.True(starting.IsSuccess);

        Result opening = await _api.ExecuteCommand(
            new OpenPageCommand(
                "https://www.avito.ru/all/gruzoviki_i_spetstehnika/pogruzchiki-ASgBAgICAURU4E0"
            )
        );
        Assert.True(opening.IsSuccess);

        Result bottomScrolling = await _api.ExecuteCommand(new ScrollToDownCommand());
        Assert.True(bottomScrolling.IsSuccess);

        Result topScrolling = await _api.ExecuteCommand(new ScrollToTopCommand());
        Assert.True(topScrolling.IsSuccess);

        Result stopping = await _api.ExecuteCommand(new StopWebDriverCommand());
        Assert.True(stopping.IsSuccess);
    }
}
