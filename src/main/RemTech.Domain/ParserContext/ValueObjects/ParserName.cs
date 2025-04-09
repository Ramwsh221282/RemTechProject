using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext.ValueObjects;

public sealed record ParserName
{
    public const int MAX_LENGTH = 30;
    public string Value { get; }

    private ParserName(string value) => Value = value;

    public static Result<ParserName> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ErrorFactory.EmptyOrNull(nameof(ParserName));

        if (name.Length > MAX_LENGTH)
            return ErrorFactory.ExceesLength(nameof(ParserName), MAX_LENGTH);

        return new ParserName(name);
    }
}
