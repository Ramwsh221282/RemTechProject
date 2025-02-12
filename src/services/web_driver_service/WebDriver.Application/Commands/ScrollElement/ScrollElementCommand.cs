using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.ScrollElement;

public sealed record ScrollElementCommand(ExistingElementDTO Data) : IWebDriverCommand;

internal sealed class ScrollElementCommandHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverCommandHandler<ScrollElementCommand>
{
    public async Task<Result> Handle(ScrollElementCommand command)
    {
        ExistingElementDTO data = command.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy strategy = InteractionStrategyFactory.ScrollElement(data.ExistingId);
        Result interaction = await instance.PerformInteraction(strategy);

        return interaction.IsFailure ? interaction.LogAndReturn(logger) : interaction;
    }
}
