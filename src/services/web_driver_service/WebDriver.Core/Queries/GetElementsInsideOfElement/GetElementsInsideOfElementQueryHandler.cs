using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Core;

namespace WebDriver.Core.Queries.GetElementsInsideOfElement;

public sealed class GetElementsInsideOfElementQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetElementsInsideOfElementQuery, WebElementObject[]>
{
    private readonly GetElementsInsideOfElementQueryValidator _validator;

    public GetElementsInsideOfElementQueryHandler(
        WebDriverInstance instance,
        ILogger logger,
        GetElementsInsideOfElementQueryValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result<WebElementObject[]>> Execute(GetElementsInsideOfElementQuery query)
    {
        var validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
        {
            Error error = validation.ToError();
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject[]> elements = _instance.GetMultipleElements(
            query.ExistingId,
            query.Requested
        );
        if (elements.IsFailure)
        {
            Error error = elements.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Found {count} elements with search type: {Type} and path: {Path}",
            elements.Value.Length,
            query.Requested.Type,
            query.Requested.Path
        );
        return elements.Value;
    }
}
