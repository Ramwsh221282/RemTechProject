using System.Runtime.CompilerServices;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.StartWebDriver;

public sealed record StartWebDriverCommand(DriverStartDataDTO Data) : IWebDriverCommand;

internal sealed class StartWebDriverCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<StartWebDriverCommand>
{
    public async Task<Result> Handle(StartWebDriverCommand command)
    {
        DriverStartDataDTO data = command.Data;
        IInteractionStrategy strategy = InteractionStrategyFactory.Start(_logger, data.Strategy);
        Result starting = await _instance.PerformInteraction(strategy);
        if (starting.IsFailure)
        {
            Error error = starting.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Web Driver Has been started with strategy: {Strategy}", data.Strategy);
        return starting;
    }
}
