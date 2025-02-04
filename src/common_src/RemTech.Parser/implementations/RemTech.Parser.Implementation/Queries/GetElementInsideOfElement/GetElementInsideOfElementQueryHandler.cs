using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Queries;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Queries.GetElementInsideOfElement;

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

        Result<WebElementObjectInternal> existingElement = _instance.GetExistingElement(
            query.Element.Position
        );
        if (existingElement.IsFailure)
        {
            Error error = existingElement.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject> element = _instance.GetElement(existingElement, query.Query);
        if (element.IsFailure)
        {
            Error error = element.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Child element with path: {ChildPath} and type: {ChildType} was found in parent element with path: {ParentPath} and type: {ParentType}",
            query.Query.Path,
            query.Query.Type,
            query.Element.ElementPath,
            query.Element.ElementPathType
        );
        return await Task.FromResult(element);
    }
}
