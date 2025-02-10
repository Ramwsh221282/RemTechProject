using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.StopWebDriver;

public sealed record StopWebDriverCommand : IWebDriverCommand;

internal sealed class StopWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<StopWebDriverCommand>
{
    public async Task<Result> Handle(StopWebDriverCommand command)
    {
        IInteractionStrategy strategy = InteractionStrategyFactory.Stop();
        Result stopping = await _instance.PerformInteraction(strategy);
        if (stopping.IsFailure)
        {
            Error error = stopping.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Web Driver has been stopped");
        return stopping;
    }
}
