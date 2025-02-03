using RemTechCommon;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record Title
{
    public string Text { get; }

    private Title(string text) => Text = text;

    public static Result<Title> Create(string? text) =>
        string.IsNullOrWhiteSpace(text) ? new Error("Title is empty") : new Title(text);
}
