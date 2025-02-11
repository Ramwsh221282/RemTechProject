using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetPageHtml;

public record GetPageHtmlQuery : IWebDriverQuery<string>;

internal sealed class GetPageHtmlQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetPageHtmlQuery, string>
{
    public GetPageHtmlQueryHandler(WebDriverInstance instance, ILogger logger)
        : base(instance, logger) { }

    public async Task<Result<string>> Execute(GetPageHtmlQuery query)
    {
        IInteractionStrategy<string> interaction = InteractionStrategyFactory.ExtractHtml();
        Result<string> html = await _instance.PerformInteraction(interaction);
        if (html.IsFailure)
        {
            Error error = html.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information("Extract html query completed");
        return html;
    }
}
