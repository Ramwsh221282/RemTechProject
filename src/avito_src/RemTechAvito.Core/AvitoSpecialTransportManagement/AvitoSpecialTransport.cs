using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class AvitoSpecialTransport
{
    public AvitoId Id { get; }
    public AddressInfo Address { get; }
    public Characteristics Characteristics { get; }
    public Description Description { get; }
    public SourceUrl Url { get; }
    public Title Title { get; }

    public PhotoDetails Photos { get; } = PhotoDetails.Empty();

    private AvitoSpecialTransport() { } // ef core

    public AvitoSpecialTransport(
        AvitoId id,
        AddressInfo address,
        Characteristics characteristics,
        Description description,
        SourceUrl url,
        Title title
    )
    {
        Id = id;
        Address = address;
        Characteristics = characteristics;
        Description = description;
        Url = url;
        Title = title;
    }
}
