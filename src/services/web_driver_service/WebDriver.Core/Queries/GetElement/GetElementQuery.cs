using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Core;

namespace WebDriver.Core.Queries.GetElement;

public abstract record GetElementQuery(string Path, string Type) : IQuery<WebElementObject>;

internal sealed record GetElementByXPathQuery(string Path) : GetElementQuery(Path, "xpath");

internal sealed record GetElementByClassQuery(string Path) : GetElementQuery(Path, "class");

internal sealed record GetElementByTagQuery(string TagName) : GetElementQuery(TagName, "tag");

public static class GetElementQueryFactory
{
    public static Result<GetElementQuery> Create(string Path, string Type)
    {
        if (string.IsNullOrWhiteSpace(Path))
            return new Error("Element path is required.");
        if (string.IsNullOrWhiteSpace(Type))
            return new Error("Element type is required.");
        return Type switch
        {
            "xpath" => new GetElementByXPathQuery(Path),
            "class" => new GetElementByClassQuery(Path),
            "tag" => new GetElementByTagQuery(Path),
            _ => new Error("Unknown element query type"),
        };
    }
}
