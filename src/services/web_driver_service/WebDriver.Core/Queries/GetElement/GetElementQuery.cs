using RemTech.WebDriver.Plugin.Core;

namespace RemTech.WebDriver.Plugin.Queries.GetElement;

internal abstract record GetElementQuery(string Path, string Type) : IQuery<WebElementObject>;

internal record GetElementByXPathQuery(string Path) : GetElementQuery(Path, "xpath");

internal record GetElementByClassQuery(string Path) : GetElementQuery(Path, "class");

internal record GetElementByTagQuery(string TagName) : GetElementQuery(TagName, "tag");
