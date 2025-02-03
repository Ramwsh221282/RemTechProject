namespace RemTech.Parser.Contracts.Contracts.Commands;

public sealed record OpenPageCommand(string? PageUrl) : IWebDriverCommand;
