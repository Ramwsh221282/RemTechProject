namespace RemTech.WebDriver.Plugin.Commands.OpenPage;

internal sealed record OpenPageCommand(string? PageUrl) : ICommand;
