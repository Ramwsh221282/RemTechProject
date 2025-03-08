namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record RecordId
{
    public Guid Value { get; }

    private RecordId(Guid id) => Value = id;

    public static RecordId CreateNew() => new RecordId(Guid.NewGuid());

    public static RecordId CreateExisting(Guid id) => new RecordId(id);

    public static RecordId CreateEmpty() => new RecordId(Guid.Empty);
}
