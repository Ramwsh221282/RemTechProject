using RemTech.WebDriver.Plugin.Core;
using RemTech.WebDriver.Plugin.Queries.GetElement;

namespace RemTech.WebDriver.Plugin.Queries.GetElementInsideOfElement;

internal record GetElementInsideOfElementQuery(GetElementQuery Query, WebElementObject Element)
    : IQuery<WebElementObject>;
