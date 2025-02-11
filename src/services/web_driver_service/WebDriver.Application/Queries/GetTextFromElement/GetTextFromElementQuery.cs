using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetTextFromElement;

public sealed record GetTextFromElementQuery(ExistingElementDTO Data) : IWebDriverQuery<string>;

internal sealed class GetTextElementFromQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverQueryHandler<GetTextFromElementQuery, string>
{
    public async Task<Result<string>> Execute(GetTextFromElementQuery query)
    {
        ExistingElementDTO data = query.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy<string> extraction = InteractionStrategyFactory.ExtractText(
            data.ExistingId
        );

        Result<string> text = await instance.PerformInteraction(extraction);
        if (text.IsFailure)
            return text.LogAndReturn(logger);

        logger.Information(
            "Text from element ({Id}) has been extracted: {Text}",
            data.ExistingId,
            text.Value
        );
        return text;
    }
}
