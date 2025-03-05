using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record Title
{
    public string Value { get; }
    private Title(string title) => Value = title;

    public static Result<Title> Create(string value) =>
        string.IsNullOrWhiteSpace(value) ? new Error("Title is empty") : new Title(value);
}