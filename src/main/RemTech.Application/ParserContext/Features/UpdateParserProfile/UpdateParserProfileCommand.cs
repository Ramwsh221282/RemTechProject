using RemTech.Application.ParserContext.Dtos;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile;

public sealed record UpdateParserProfileCommand(
    string ParserName,
    string ProfileName,
    UpdateParserProfileDto Data
) : ICommand;
