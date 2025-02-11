namespace WebDriver.Core.Models.WebDriverStates.States;

internal sealed class NotInitializedState : IWebDriverState
{
    public string StateName { get; } = "Not Initialized";

    public bool CanExecuteAction { get; } = false;
}
