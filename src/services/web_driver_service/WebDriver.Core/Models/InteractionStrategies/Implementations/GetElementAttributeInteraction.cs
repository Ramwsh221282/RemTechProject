using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class GetElementAttributeInteraction : IInteractionStrategy<string>
{
    private readonly Guid _id;
    private readonly string _attribute;

    public GetElementAttributeInteraction(Guid id, string attribute)
    {
        _id = id;
        _attribute = attribute;
    }

    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> driver = instance.GetRunningDriver();
        if (driver.IsFailure)
            return driver.Error;

        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return element.Error;

        IWebElement model = element.Value.Model;
        try
        {
            string value = model.GetAttribute(_attribute);
            return await Task.FromResult(value);
        }
        catch (Exception ex)
        {
            return new Error(
                $"Cannot get attribute: {_attribute} of element with id: {_id}. Error:{ex.Message}"
            );
        }
    }
}
