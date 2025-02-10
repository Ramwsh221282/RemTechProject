using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.ClickOnElement;

public record ClickOnElementCommand(ExistingElementDTO Data) : IWebDriverCommand;

internal sealed class ClickOnElementCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<ClickOnElementCommand>
{
    public async Task<Result> Handle(ClickOnElementCommand command)
    {
        ExistingElementDTO data = command.Data;
        IInteractionStrategy strategy = InteractionStrategyFactory.Click(data.ExistingId);
        Result clicking = await _instance.PerformInteraction(strategy);
        if (clicking.IsFailure)
        {
            Error error = clicking.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Performed click on element({ID})", data.ExistingId);
        return clicking;
    }
}
