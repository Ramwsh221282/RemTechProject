using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record Title
{
    public string Text { get; }

    private Title(string text) => Text = text;

    public static Result<Title> Create(string? title) =>
        string.IsNullOrWhiteSpace(title) ? new Error("Title cannot be empty") : new Title(title);
}
