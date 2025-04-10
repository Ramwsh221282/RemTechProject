namespace RemTech.Application.ParserContext.Dtos;

public record UpdateParserProfileDto(
    string? ProfileName,
    string? ProfileState,
    string[]? Links,
    ParserScheduleDto? Schedule
);
