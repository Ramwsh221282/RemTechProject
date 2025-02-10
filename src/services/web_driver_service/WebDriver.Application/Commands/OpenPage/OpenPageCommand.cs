using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Commands.OpenPage;

public sealed record OpenPageCommand(WebPageDataDTO Data) : IWebDriverCommand;

internal sealed class OpenPageCommandHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverCommandHandler<OpenPageCommand>
{
    public async Task<Result> Handle(OpenPageCommand command)
    {
        WebPageDataDTO data = command.Data;
        IInteractionStrategy<string> strategy = InteractionStrategyFactory.CreateOpenPage(
            data.PageUrl
        );

        Result<string> opening = await _instance.PerformInteraction(strategy);
        if (opening.IsFailure)
        {
            Error error = opening.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Web Driver Has opened page with url: {Url}", data.PageUrl);
        return opening;
    }
}
