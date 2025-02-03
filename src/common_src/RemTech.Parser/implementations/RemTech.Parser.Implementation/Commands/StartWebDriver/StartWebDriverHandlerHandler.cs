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
            return new Error("Web driver is not created").LogAndReturn(_logger);

        Result result = _instance.Instantiate();
        return await Task.FromResult(
            result.IsFailure
                ? result.Error.LogAndReturn(_logger)
                : result.LogAndReturn(_logger, "Started Web Driver Instance")
        );
    }
}
