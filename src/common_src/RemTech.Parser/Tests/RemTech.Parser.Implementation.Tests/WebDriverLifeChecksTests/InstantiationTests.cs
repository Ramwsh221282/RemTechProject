using Microsoft.Extensions.DependencyInjection;
using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Implementation.Injection;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Tests.WebDriverLifeChecksTests;

public sealed class InstantiationTests
{
    private readonly IServiceProvider _provider;

    public InstantiationTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCommonInjections();
        services.InjectWebDriverComponents();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public void WebDriver_Factory_Instantiation_Test()
    {
        IWebDriverFactory factory = _provider.GetRequiredService<IWebDriverFactory>();
        Result<IWebDriverInstance> instantiation = factory.Create();
        Assert.True(instantiation.IsSuccess);
        Assert.NotNull(instantiation.Value);
        instantiation.Value.Dispose();
        Assert.True(instantiation.Value.IsDisposed);
    }

    [Fact]
    public void WebDriver_Installation_Test()
    {
        IWebDriverExecutableManager manager =
            _provider.GetRequiredService<IWebDriverExecutableManager>();
        Result<string> installation = manager.Install();
        Assert.EndsWith("chromedriver.exe", installation.Value);
        Assert.True(installation.IsSuccess);
    }

    [Fact]
    public void WebDriver_Uninstallation_Test()
    {
        IWebDriverExecutableManager manager =
            _provider.GetRequiredService<IWebDriverExecutableManager>();
        Result<string> uninstallation = manager.Uninstall();
        Assert.True(uninstallation.IsSuccess);
    }
}
