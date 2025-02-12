using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.ClearText;

public sealed record ClearTextCommand(ExistingElementDTO Data) : IWebDriverCommand;

internal sealed class ClearTextCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverCommandHandler<ClearTextCommand>
{
    public async Task<Result> Handle(ClearTextCommand command)
    {
        ExistingElementDTO data = command.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy strategy = InteractionStrategyFactory.ClearText(data.ExistingId);
        Result interaction = await instance.PerformInteraction(strategy);

        if (interaction.IsFailure)
            return interaction.Error;

        IInteractionStrategy<string> checking = InteractionStrategyFactory.ExtractText(
            data.ExistingId
        );
        Result<string> checkingText = await instance.PerformInteraction(checking);
        if (checkingText.IsFailure)
            return checkingText.Error;

        string text = checkingText.Value;
        if (!string.IsNullOrWhiteSpace(text))
        {
            logger.Error("Unable clear text from element ({ID})", data.ExistingId);
            return new Error($"Unable clear text from element({data.ExistingId})");
        }

        logger.Information("Cleared text from element ({ID})", data.ExistingId);
        return Result.Success();
    }
}
