using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.ScrollToDown;

public sealed record ScrollToDownCommand : IWebDriverCommand;

internal sealed class ScrollToDownCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<ScrollToDownCommand>
{
    public async Task<Result> Handle(ScrollToDownCommand command)
    {
        IInteractionStrategy strategy = InteractionStrategyFactory.CreateScrollToBottom();
        Result interaction = await _instance.PerformInteraction(strategy);
        if (interaction.IsFailure)
        {
            Error error = interaction.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }
        _logger.Information("Web Driver has scrolled page to bottom");
        return interaction;
    }
}
