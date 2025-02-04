namespace RemTech.Parser.Contracts.Contracts.Queries;

public record GetElementsInsideOfElementQuery(WebElementObject Parent, GetElementQuery Query)
    : IWebDriverQuery<WebElementObject[]>;
