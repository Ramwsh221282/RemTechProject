using WebDriver.Core.Core;
using WebDriver.Core.Queries.GetElement;

namespace WebDriver.Core.Queries.GetElementInsideOfElement;

public record GetElementInsideOfElementQuery(Guid ExistingId, GetElementQuery Requested)
    : IQuery<WebElementObject>;
