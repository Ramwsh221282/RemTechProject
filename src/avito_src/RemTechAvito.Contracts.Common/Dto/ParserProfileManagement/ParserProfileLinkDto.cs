namespace RemTechAvito.Contracts.Common.Dto.ParserProfileManagement;

public sealed record ParserProfileDto(string Id, bool State, ParserProfileLinkDto[] Links);

public sealed record ParserProfileLinkDto(string Mark, string Link, string? Id = null);
