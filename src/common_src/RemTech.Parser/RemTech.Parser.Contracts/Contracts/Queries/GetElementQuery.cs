namespace RemTech.Parser.Contracts.Contracts.Queries;

public abstract record GetElementQuery(string Path, string Type)
    : IWebDriverQuery<WebElementObject>;

public record GetElementByXPathQuery(string Path) : GetElementQuery(Path, "xpath");

public record GetElementByClassQuery(string Path) : GetElementQuery(Path, "class");

public record GetElementByTagQuery(string TagName) : GetElementQuery(TagName, "tag");
