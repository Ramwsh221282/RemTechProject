using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Application.DTO;
using WebDriver.Application.Extensions;
using WebDriver.Application.Handlers;
using WebDriver.Core.Models;
using WebDriver.Core.Models.InteractionStrategies;

namespace WebDriver.Application.Queries.GetElementHtml;

public sealed record GetElementHtmlQuery(ExistingElementDTO Data) : IWebDriverQuery<string>;

internal sealed class GetElementHtmlQueryHandler(
    WebDriverInstance instance,
    ILogger logger,
    ExistingElementDTOValidator validator
) : IWebDriverQueryHandler<GetElementHtmlQuery, string>
{
    public async Task<Result<string>> Execute(GetElementHtmlQuery query)
    {
        ExistingElementDTO data = query.Data;
        ValidationResult validation = await validator.ValidateAsync(data);
        if (!validation.IsValid)
            return validation.LogAndReturn(logger);

        IInteractionStrategy<string> strategy = InteractionStrategyFactory.ExtractHtml(
            data.ExistingId
        );
        Result<string> html = await instance.PerformInteraction(strategy);
        if (html.IsFailure)
            return html.LogAndReturn(logger);

        logger.Information("Extracted html from element({Id})", data.ExistingId);
        return html;
    }
}
