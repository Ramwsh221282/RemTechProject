using RemTech.Parser.Contracts.Contracts.Commands;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Commands.StopWebDriver;

public sealed class StopWebDriverCommandHandler
    : BaseWebDriverCommand,
        IWebDriverCommandHandler<StopWebDriverCommand>
{
    public StopWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result> Handle(StopWebDriverCommand command)
    {
        Result result = _instance.Dispose();
        return await Task.FromResult(
            result.IsFailure
                ? result.Error.LogAndReturn(_logger)
                : result.LogAndReturn(_logger, "Web Driver Instance Stopped")
        );
    }
}
