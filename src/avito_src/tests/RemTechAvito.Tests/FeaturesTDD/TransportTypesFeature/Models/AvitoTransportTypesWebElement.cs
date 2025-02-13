using WebDriver.Worker.Service.Contracts.Responses;

namespace RemTechAvito.Tests.FeaturesTDD.TransportTypesFeature.Models;

public sealed class AvitoTransportTypesWebElement
{
    public const string InputElementPath =
        ".//input[@data-marker='params[111024]/multiselect-outline-input/input']";
    public const string InputElementPathType = "xpath";
    public const string ElementsListContainerPath = ".//div[@data-marker='params[111024]/list']";
    public const string ElementCheckBoxPath = ".//label[@role='checkbox']";
    public const string CheckBoxCheckedAttribute = "aria-checked";

    public string Name { get; set; } = string.Empty;
    public WebElementResponse? Element { get; set; } = null;
}
