using Serilog;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;
using Result = RemTechCommon.Utils.ResultPattern.Result;

namespace WebDriver.Application.Commands.ScrollToTop;

public sealed record ScrollToTopCommand : IWebDriverCommand;

internal sealed class ScrollToTopCommandHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverCommandHandler<ScrollToTopCommand>
{
    public async Task<Result> Handle(ScrollToTopCommand command)
    {
        IInteractionStrategy strategy = InteractionStrategyFactory.CreateScrollTop();
        Result scrolling = await instance.PerformInteraction(strategy);
        if (scrolling.IsFailure)
            return scrolling.LogAndReturn(logger);

        logger.Information("Web Driver has scrolled page to top");
        return scrolling;
    }
}
