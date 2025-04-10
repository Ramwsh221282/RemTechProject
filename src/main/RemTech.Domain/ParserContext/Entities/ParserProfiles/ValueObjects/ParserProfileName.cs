using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

public sealed record ParserProfileName
{
    public const int MAX_NAME_LENGTH = 30;
    public string Value { get; }

    private ParserProfileName(string value) => Value = value;

    public static Result<ParserProfileName> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ErrorFactory.EmptyOrNull(nameof(ParserProfileName));

        if (name.Length > MAX_NAME_LENGTH)
            return ErrorFactory.ExceesLength(nameof(ParserProfileName), MAX_NAME_LENGTH);

        return new ParserProfileName(name);
    }
}
