using FluentValidation;
using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.ClickOnElement;

public record ClickOnElementCommand(ExistingElementDTO Data) : IWebDriverCommand;

internal sealed class ClickOnElementCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverCommandHandler<ClickOnElementCommand>
{
    public async Task<Result> Handle(ClickOnElementCommand command)
    {
        ExistingElementDTO data = command.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy strategy = InteractionStrategyFactory.Click(data.ExistingId);
        Result clicking = await instance.PerformInteraction(strategy);
        if (clicking.IsFailure)
            return clicking.LogAndReturn(logger);

        logger.Information("Performed click on element({ID})", data.ExistingId);
        return clicking;
    }
}
