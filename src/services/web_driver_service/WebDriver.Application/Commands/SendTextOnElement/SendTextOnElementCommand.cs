using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.SendTextOnElement;

public sealed record SendTextOnElementCommand(ExistingElementDTO Data, string Text)
    : IWebDriverCommand;

internal sealed class SendTextOnElementCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverCommandHandler<SendTextOnElementCommand>
{
    public async Task<Result> Handle(SendTextOnElementCommand command)
    {
        ExistingElementDTO data = command.Data;
        string text = command.Text;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy<string> strategy = InteractionStrategyFactory.SendText(
            data.ExistingId,
            text
        );

        Result<string> input = await instance.PerformInteraction(strategy);
        if (input.IsFailure)
            return input.LogAndReturn(logger);

        logger.Information("Input text was successfull. Input: {Input}", input.Value);
        return input;
    }
}
