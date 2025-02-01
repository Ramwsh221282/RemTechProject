using RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;
using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement;

public sealed class AvitoAuthor
{
    private readonly List<AvitoSpecialTransport> _transport = [];
    public AvitoParsedAuthorId ParsedAuthorId { get; }
    public AvitoAuthorId Id { get; }
    public AuthorDetails Details { get; }
    public AuthorCategory Category { get; }
    public IReadOnlyCollection<AvitoSpecialTransport> Transport => _transport;

    private AvitoAuthor() { } // ef core

    public AvitoAuthor(
        AvitoParsedAuthorId parsedAuthorId,
        AuthorDetails details,
        AuthorCategory category
    )
    {
        ParsedAuthorId = parsedAuthorId;
        Details = details;
        Category = category;
        Id = new AvitoAuthorId(new RandomGuidGenerator());
    }

    public Result AddTransport(AvitoSpecialTransport transport)
    {
        Result has = HasTransport((item) => item.ParsedId == transport.ParsedId);
        if (has.IsSuccess)
            return new Error("Author has information about this transport already.");
        _transport.Add(transport);
        return Result.Success();
    }

    public Result HasTransport(Func<AvitoSpecialTransport, bool> predicate)
    {
        bool has = _transport.Any(predicate);
        return has
            ? Result.Success()
            : Result.Failure(new Error("Does not have transport with condition"));
    }
}
