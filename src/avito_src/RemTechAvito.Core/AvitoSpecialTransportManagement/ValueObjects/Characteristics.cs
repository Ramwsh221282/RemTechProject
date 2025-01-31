using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public record Characteristics
{
    public string Text { get; }

    private Characteristics(string text) => Text = text;

    public static Result<Characteristics> Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Characteristics were empty");
        return new Characteristics(text);
    }
}
