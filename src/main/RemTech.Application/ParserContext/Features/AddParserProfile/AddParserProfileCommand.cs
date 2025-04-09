namespace RemTech.Application.ParserContext.Features.AddParserProfile;

public sealed record AddParserProfileCommand(string ParserName, string ProfileName) : ICommand;
