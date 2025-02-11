namespace WebDriver.Core.Models.WebDriverStates;

internal interface IWebDriverState
{
    public string StateName { get; }
    public bool CanExecuteAction { get; }
}
