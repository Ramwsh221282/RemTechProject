using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;
using WebDriver.Core.Core;

namespace WebDriver.Core.Queries.GetElementInsideOfElement;

public sealed class GetElementInsideOfElementQueryHandler
    : BaseWebDriverHandler,
        IWebDriverQueryHandler<GetElementInsideOfElementQuery, WebElementObject>
{
    private readonly GetElementInsideOfElementQueryValidator _validator;

    public GetElementInsideOfElementQueryHandler(
        WebDriverInstance instance,
        ILogger logger,
        GetElementInsideOfElementQueryValidator validator
    )
        : base(instance, logger) => _validator = validator;

    public async Task<Result<WebElementObject>> Execute(GetElementInsideOfElementQuery query)
    {
        var validation = await _validator.ValidateAsync(query);
        if (!validation.IsValid)
        {
            Error error = validation.ToError();
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject> element = _instance.GetSingleElement(
            query.ExistingId,
            query.Requested
        );
        if (element.IsFailure)
        {
            Error error = element.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Children elements (Path: {ChildPath} Type: {ChildType}) found",
            query.Requested.Path,
            query.Requested.Type
        );
        return element;
    }
}
