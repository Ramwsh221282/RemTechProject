using RemTech.Domain.AdvertisementsContext.ValueObjects;

namespace RemTech.Application.AdvertisementsContext.Models.CharacteristicsManagement;

public sealed class TransportCharacteristic
{
    public Guid Id { get; }
    public string Name { get; }
    public string Value { get; }

    private TransportCharacteristic()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Value = string.Empty;
    } // ef core

    public TransportCharacteristic(AdvertisementCharacteristic advertisementCharacteristic)
    {
        Id = Guid.NewGuid();
        Name = advertisementCharacteristic.Name;
        Value = advertisementCharacteristic.Value;
    }
}
