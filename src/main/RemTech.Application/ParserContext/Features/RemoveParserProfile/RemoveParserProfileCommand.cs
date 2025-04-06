using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Application.ParserContext.Features.RemoveParserProfile;

public sealed record RemoveParserProfileCommand(string ParserName, string ProfileName)
    : ICommand<UnitResult<Guid>>;
