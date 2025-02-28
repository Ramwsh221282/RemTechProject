namespace RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;

public sealed record ParserProfileResponse(
    Guid Id,
    DateOnly CreatedOn,
    string Name,
    bool State,
    string StateDescription,
    IEnumerable<ParserProfileLinksResponse> Links
);

public sealed record ParserProfileLinksResponse(
    string Name,
    string Link,
    IEnumerable<string> Additions
);
