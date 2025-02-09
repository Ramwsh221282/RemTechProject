using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetElement;

namespace WebDriver.Core.Queries.GetElementsInsideOfElement;

public record GetElementsInsideOfElementQuery(Guid ExistingId, GetElementQuery Requested)
    : IQuery<WebElementObject[]>;
