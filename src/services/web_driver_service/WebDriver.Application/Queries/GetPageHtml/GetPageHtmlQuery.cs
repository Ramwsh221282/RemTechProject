using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetPageHtml;

public record GetPageHtmlQuery : IWebDriverQuery<string>;

internal sealed class GetPageHtmlQueryHandler(WebDriverInstance instance, ILogger logger)
    : IWebDriverQueryHandler<GetPageHtmlQuery, string>
{
    public async Task<Result<string>> Execute(GetPageHtmlQuery query)
    {
        IInteractionStrategy<string> interaction = InteractionStrategyFactory.ExtractHtml();
        Result<string> html = await instance.PerformInteraction(interaction);
        if (html.IsFailure)
            return html.LogAndReturn(logger);

        logger.Information("Extract html query completed");
        return html;
    }
}
