namespace RemTech.Application.ParserContext.Features.RemoveParserProfile;

public sealed record RemoveParserProfileCommand(string ParserName, string ProfileName) : ICommand;
