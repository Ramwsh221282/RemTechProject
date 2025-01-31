using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class AvitoAuthor
{
    private readonly List<AvitoSpecialTransport> _transport = [];
    public AvitoAuthorId Id { get; }
    public AuthorDetails Details { get; }
    public AuthorCategory Category { get; }
    public IReadOnlyCollection<AvitoSpecialTransport> Transport => _transport;

    private AvitoAuthor() { } // ef core

    public AvitoAuthor(AvitoAuthorId id, AuthorDetails details, AuthorCategory category)
    {
        Id = id;
        Details = details;
        Category = category;
    }

    public void AddTransport(AvitoSpecialTransport transport)
    {
        _transport.Add(transport);
    }
}
