using RemTechCommon;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record Description
{
    public string Text { get; }

    private Description(string text) => Text = text;

    public static Result<Description> Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Description cannot be empty");
        return new Description(text);
    }
}
