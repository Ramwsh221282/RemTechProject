using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.ParserProfileManagement.ValueObjects;

public sealed record ParserProfileName
{
    public string Name { get; }

    private ParserProfileName(string name)
    {
        Name = name;
    }

    public static Result<ParserProfileName> Create(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? new Error("Parser profile name cannot be empty")
            : new ParserProfileName(name);
    }
}
