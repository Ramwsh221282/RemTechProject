using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.Common.ValueObjects;

public sealed record Description
{
    public string Text { get; }

    private Description(string text) => Text = text;

    public static Result<Description> Create(string? text) =>
        string.IsNullOrWhiteSpace(text)
            ? new Error("Description text should be provided")
            : new Description(text);
}
