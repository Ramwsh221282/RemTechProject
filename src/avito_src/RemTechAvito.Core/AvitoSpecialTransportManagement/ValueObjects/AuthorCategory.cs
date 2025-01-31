namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AuthorCategory
{
    public string Text { get; }

    private AuthorCategory(string text) => Text = text;

    public static AuthorCategory CreateCommercial() => new("Организация");

    public static AuthorCategory CreatePerson() => new("Физ. лицо");
}
