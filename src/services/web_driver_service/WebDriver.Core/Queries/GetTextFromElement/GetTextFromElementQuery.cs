using FluentValidation.Results;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Core;

namespace WebDriver.Core.Queries.GetTextFromElement;

public sealed record GetTextFromElementQuery(Guid ExistingId)
    : IQuery<GetTextFromElementQueryResult>;

public sealed record GetTextFromElementQueryResult(string Text);

public sealed class GetTextElementFromQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetTextFromElementQuery, GetTextFromElementQueryResult>
{
    private const string GetTextFromElementScript = "return arguments[0].innerText;";
    private readonly GetTextFromElementQueryValidator _validator;

    public GetTextElementFromQueryHandler(
        WebDriverInstance instance,
        ILogger logger,
        GetTextFromElementQueryValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result<GetTextFromElementQueryResult>> Execute(GetTextFromElementQuery query)
    {
        if (_instance.Instance == null || _instance.IsDisposed)
        {
            Error error = WebDriverPluginErrors.Disposed;
            _logger.Error("{Error}", error);
            return error;
        }

        ValidationResult validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
        {
            Error error = validation.ToError();
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject> existingElement = _instance.GetFromPool(query.ExistingId);
        if (existingElement.IsFailure)
        {
            Error error = existingElement.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        string text = _instance.Instance.ExecuteJavaScript<string>(
            "return arguments[0].innerText;",
            existingElement.Value.Model
        )!;

        GetTextFromElementQueryResult result = new(text);
        _logger.Information("Got text from element: {Text}", text);
        return result;
    }
}
