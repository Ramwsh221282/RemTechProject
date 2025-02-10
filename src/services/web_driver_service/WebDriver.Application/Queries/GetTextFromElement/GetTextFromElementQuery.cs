using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetTextFromElement;

public sealed record GetTextFromElementQuery(ExistingElementDTO Data) : IWebDriverQuery<string>;

internal sealed class GetTextElementFromQueryHandler(WebDriverInstance instance, ILogger logger)
    : BaseWebDriverHandler(instance, logger),
        IWebDriverQueryHandler<GetTextFromElementQuery, string>
{
    public async Task<Result<string>> Execute(GetTextFromElementQuery query)
    {
        ExistingElementDTO data = query.Data;
        IInteractionStrategy<string> extraction = InteractionStrategyFactory.ExtractText(
            data.ExistingId
        );
        Result<string> text = await _instance.PerformInteraction(extraction);
        if (text.IsFailure)
        {
            Error error = text.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Text from element ({Id}) has been extracted: {Text}",
            data.ExistingId,
            text.Value
        );
        return text;
    }
}
