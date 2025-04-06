using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Application.ParserContext.Features.AddParserProfile;

public sealed record AddParserProfileCommand(string ParserName, string ProfileName)
    : ICommand<UnitResult<Guid>>;
