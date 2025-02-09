namespace WebDriver.Core.Commands.OpenPage;

public sealed record OpenPageCommand(string? PageUrl) : ICommand;
