using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;
using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class AvitoSpecialTransport
{
    public AvitoSpecialTransportId Id { get; }
    public AvitoId ParsedId { get; }
    public AvitoAuthorId AuthorId { get; }
    public TransportMarkId MarkId { get; }
    public TransportStateId StateId { get; }
    public TransportTypeId TypeId { get; }
    public AddressInfo Address { get; }
    public Characteristics Characteristics { get; }
    public Description Description { get; }
    public SourceUrl Url { get; }
    public Title Title { get; }

    public PhotoDetails Photos { get; } = PhotoDetails.Empty();

    private AvitoSpecialTransport() { } // ef core

    public AvitoSpecialTransport(
        AvitoId parsedId,
        AvitoAuthorId authorId,
        TransportMarkId markId,
        TransportStateId stateId,
        TransportTypeId typeId,
        AddressInfo address,
        Characteristics characteristics,
        Description description,
        SourceUrl url,
        Title title
    )
    {
        ParsedId = parsedId;
        AuthorId = authorId;
        MarkId = markId;
        StateId = stateId;
        TypeId = typeId;
        Address = address;
        Characteristics = characteristics;
        Description = description;
        Url = url;
        Title = title;
        Id = new AvitoSpecialTransportId(new RandomGuidGenerator());
    }
}
