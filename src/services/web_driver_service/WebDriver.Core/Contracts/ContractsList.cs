using RemTech.WebDriver.Plugin.Commands.OpenPage;
using RemTech.WebDriver.Plugin.Commands.ScrollToDown;
using RemTech.WebDriver.Plugin.Commands.ScrollToTop;
using RemTech.WebDriver.Plugin.Commands.StartWebDriver;
using RemTech.WebDriver.Plugin.Commands.StopWebDriver;
using RemTech.WebDriver.Plugin.Queries.GetElement;
using RemTech.WebDriver.Plugin.Queries.GetElementInsideOfElement;
using RemTech.WebDriver.Plugin.Queries.GetElementsInsideOfElement;

namespace RemTech.WebDriver.Plugin.Contracts;

internal static class ContractsList
{
    public const string OpenPage = nameof(OpenPageCommand);
    public const string ScrollToDown = nameof(ScrollToDownCommand);
    public const string ScrollToTop = nameof(ScrollToTopCommand);
    public const string StartWebDriver = nameof(StartWebDriverCommand);
    public const string StopWebDriver = nameof(StopWebDriverCommand);
    public const string GetElement = nameof(GetElementQuery);
    public const string GetElementInsideOfElement = nameof(GetElementInsideOfElementQuery);
    public const string GetElementsInsideOfElement = nameof(GetElementsInsideOfElementQuery);
}
