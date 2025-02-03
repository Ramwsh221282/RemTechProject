namespace RemTech.Parser.Contracts.Contracts.Queries;

public record GetElementQuery(string Path, string Type) : IWebDriverQuery<WebElementObject>;

public record GetElementByXPathQuery(string Path) : GetElementQuery(Path, "xpath");

public record GetElementByClassQuery(string Path) : GetElementQuery(Path, "class");
