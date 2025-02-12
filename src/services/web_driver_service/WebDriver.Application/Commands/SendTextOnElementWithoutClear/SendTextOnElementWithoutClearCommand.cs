using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.SendTextOnElementWithoutClear;

public sealed record SendTextOnElementWithoutClearCommand(ExistingElementDTO Data, string Text)
    : IWebDriverCommand;

internal sealed class SendTextOnElementWithoutClearCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverCommandHandler<SendTextOnElementWithoutClearCommand>
{
    public async Task<Result> Handle(SendTextOnElementWithoutClearCommand command)
    {
        ExistingElementDTO data = command.Data;
        string text = command.Text;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy strategy = InteractionStrategyFactory.SendTextNoClear(
            data.ExistingId,
            text
        );
        Result interaction = await instance.PerformInteraction(strategy);

        if (interaction.IsFailure)
            return interaction.LogAndReturn(logger);

        logger.Information("Written text in element({ID}) no clear.", data.ExistingId);
        return interaction;
    }
}
