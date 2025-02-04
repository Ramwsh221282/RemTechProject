namespace RemTech.Parser.Contracts.Contracts.Queries;

public record GetElementInsideOfElementQuery(GetElementQuery Query, WebElementObject Element)
    : IWebDriverQuery<WebElementObject>;
