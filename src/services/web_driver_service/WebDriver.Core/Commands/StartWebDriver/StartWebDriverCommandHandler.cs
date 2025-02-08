using RemTech.WebDriver.Plugin.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.WebDriver.Plugin.Commands.StartWebDriver;

internal sealed class StartWebDriverCommandHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<StartWebDriverCommand>
{
    public StartWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result> Handle(StartWebDriverCommand command)
    {
        if (_instance.Instance != null && _instance.IsDisposed == false)
        {
            Error error = new Error("Web driver is already instantiated");
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result result = _instance.Instantiate();
        if (result.IsFailure)
        {
            Error error = result.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Started Web Driver Instance");
        return await Task.FromResult(Result.Success());
    }
}
