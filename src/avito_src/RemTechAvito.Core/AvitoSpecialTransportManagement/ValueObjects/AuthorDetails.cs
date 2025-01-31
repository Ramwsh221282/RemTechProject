using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AuthorDetails
{
    public string Text { get; }

    private AuthorDetails(string text) => Text = text;

    public static Result<AuthorDetails> Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Author details cannot be empty");
        return new AuthorDetails(text);
    }
}
