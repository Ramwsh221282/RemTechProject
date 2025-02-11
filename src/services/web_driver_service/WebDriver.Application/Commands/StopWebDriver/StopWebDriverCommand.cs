using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;

namespace WebDriver.Application.Commands.StopWebDriver;

public sealed record StopWebDriverCommand : IWebDriverCommand;

internal sealed class StopWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverCommandHandler<StopWebDriverCommand>
{
    public async Task<Result> Handle(StopWebDriverCommand command)
    {
        Result stopping = instance.StopWebDriver();
        if (stopping.IsFailure)
            return stopping.LogAndReturn(logger);

        logger.Information("Web Driver has been stopped");
        return await Task.FromResult(stopping);
    }
}
