using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;

namespace WebDriver.Application.Commands.StopWebDriver;

public sealed record StopWebDriverCommand : IWebDriverCommand;

internal sealed class StopWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<StopWebDriverCommand>
{
    public async Task<Result> Handle(StopWebDriverCommand command)
    {
        Result stopping = _instance.StopWebDriver();
        if (stopping.IsFailure)
        {
            Error error = stopping.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Web Driver has been stopped");
        return await Task.FromResult(stopping);
    }
}
