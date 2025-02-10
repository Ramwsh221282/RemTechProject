using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.ScrollToTop;

public sealed record ScrollToTopCommand : IWebDriverCommand;

internal sealed class ScrollToTopCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<ScrollToTopCommand>
{
    public async Task<Result> Handle(ScrollToTopCommand command)
    {
        IInteractionStrategy strategy = InteractionStrategyFactory.CreateScrollTop();
        Result scrolling = await _instance.PerformInteraction(strategy);
        if (scrolling.IsFailure)
        {
            Error error = scrolling.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }
        _logger.Information("Web Driver has scrolled page to top");
        return scrolling;
    }
}
