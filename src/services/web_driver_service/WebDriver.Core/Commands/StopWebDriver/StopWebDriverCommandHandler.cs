using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Core;

namespace WebDriver.Core.Commands.StopWebDriver;

public sealed class StopWebDriverCommandHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<StopWebDriverCommand>
{
    public StopWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result> Handle(StopWebDriverCommand command)
    {
        Result result = _instance.Dispose();
        if (result.IsFailure)
        {
            Error error = result.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Web Driver Instance Stopped");
        return await Task.FromResult(Result.Success());
    }
}
