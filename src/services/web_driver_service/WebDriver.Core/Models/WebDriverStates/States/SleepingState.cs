namespace WebDriver.Core.Models.WebDriverStates.States;

internal sealed class SleepingState : IWebDriverState
{
    public string StateName { get; } = "Sleeping";

    public bool CanExecuteAction { get; } = true;
}
