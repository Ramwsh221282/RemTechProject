namespace RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;

public sealed record ParserProfileResponse(
    Guid Id,
    DateOnly CreatedOn,
    bool State,
    string StateDescription,
    ParserProfileLinksResponse[] Links
);

public sealed record ParserProfileLinksResponse(Guid Id, string Link, string Mark);
