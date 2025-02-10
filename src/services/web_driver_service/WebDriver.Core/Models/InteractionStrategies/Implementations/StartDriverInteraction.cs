using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class StartDriverInteraction : IInteractionStrategy
{
    private readonly ILogger _logger;
    private readonly WebDriverOptionsFactory _optionsFactory;
    private readonly WebDriverExecutableManager _manager;

    public StartDriverInteraction(ILogger logger, string loadStrategy)
    {
        _logger = logger;
        _manager = new WebDriverExecutableManager(logger);
        _optionsFactory = new(logger, loadStrategy);
    }

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        WebDriverFactory factory = new WebDriverFactory(
            _logger,
            _optionsFactory,
            _manager,
            instance
        );
        return await Task.FromResult(factory.Instantiate());
    }
}
