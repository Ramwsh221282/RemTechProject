using RemTech.WebDriver.Plugin.Core;
using RemTech.WebDriver.Plugin.Queries.GetElement;

namespace RemTech.WebDriver.Plugin.Queries.GetElementsInsideOfElement;

internal record GetElementsInsideOfElementQuery(WebElementObject Parent, GetElementQuery Query)
    : IQuery<WebElementObject[]>;
