using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.StopWebDriver;

public sealed class StopWebDriverHandlerHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<StopWebDriverCommand>
{
    public StopWebDriverHandlerHandler(WebDriverInstance instance, ILogger logger)
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
