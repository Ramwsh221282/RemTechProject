namespace WebDriver.Core.Models.WebDriverStates.States;

internal sealed class ProcessingState : IWebDriverState
{
    public string StateName { get; } = "Processing";

    public bool CanExecuteAction { get; } = false;
}
