using RemTech.Application.ParserContext.Dtos;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile;

public sealed record UpdateParserProfileCommand(
    string ParserName,
    string ProfileName,
    UpdateParserProfileDto Data
) : ICommand<UnitResult<Guid>>;
