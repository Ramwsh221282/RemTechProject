using FluentValidation.Results;
using RemTech.WebDriver.Plugin.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.WebDriver.Plugin.Queries.GetElement;

internal sealed class GetElementQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetElementQuery, WebElementObject>
{
    private readonly GetElementQueryValidator _validator;

    public GetElementQueryHandler(
        WebDriverInstance instance,
        ILogger logger,
        GetElementQueryValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result<WebElementObject>> Execute(GetElementQuery query)
    {
        ValidationResult validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
        {
            Error error = validation.ToError();
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject> result = _instance.GetElement(query);
        if (result.IsFailure)
        {
            Error error = result.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Got element with path: {Path} and type {Type}",
            query.Path,
            query.Type
        );
        return await Task.FromResult(result);
    }
}
