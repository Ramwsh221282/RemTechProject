using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.StartWebDriver;

public sealed class StartWebDriverHandlerHandler
    : BaseWebDriverHandler,
        IWebDriverCommandHandler<StartWebDriverCommand>
{
    public StartWebDriverHandlerHandler(WebDriverInstance instance, ILogger logger)
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
