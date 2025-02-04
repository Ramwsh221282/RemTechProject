using RemTech.Parser.Contracts.Contracts;
using RemTech.Parser.Contracts.Contracts.Queries;
using RemTech.Parser.Implementation.Core;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Parser.Implementation.Queries.GetElementsInsideOfElement;

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

        Result<WebElementObjectInternal> existing = _instance.GetExistingElement(query.Parent);
        if (existing.IsFailure)
        {
            Error error = existing.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        Result<WebElementObject[]> elements = _instance.GetElements(existing, query.Query);
        if (elements.IsFailure)
        {
            Error error = elements.Error;
            _logger.Error("{Error}", error.Description);
            return error;
        }

        _logger.Information(
            "Found {count} elements with search type: {Type} and path: {Path}",
            elements.Value.Length,
            query.Query.Type,
            query.Query.Path
        );
        return elements.Value;
    }
}
