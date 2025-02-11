using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.ScrollToDown;

public sealed record ScrollToDownCommand : IWebDriverCommand;

internal sealed class ScrollToDownCommandHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverCommandHandler<ScrollToDownCommand>
{
    public async Task<Result> Handle(ScrollToDownCommand command)
    {
        IInteractionStrategy strategy = InteractionStrategyFactory.CreateScrollToBottom();
        Result interaction = await instance.PerformInteraction(strategy);
        if (interaction.IsFailure)
            return interaction.LogAndReturn(logger);

        logger.Information("Web Driver has scrolled page to bottom");
        return interaction;
    }
}
