using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class SendTextOnElementWithoutClearStrategy : IInteractionStrategy
{
    private readonly Guid _id;
    private readonly string _text;

    public SendTextOnElementWithoutClearStrategy(Guid id, string text)
    {
        _id = id;
        _text = text;
    }

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> requested = instance.GetRunningDriver();
        if (requested.IsFailure)
            return requested.Error;

        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return element.Error;

        IWebDriver driver = requested.Value;
        IWebElement model = element.Value.Model;

        try
        {
            model.SendKeys(_text);
            return await Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            return new Error($"Can't write text in element: {_id}");
        }
    }
}
