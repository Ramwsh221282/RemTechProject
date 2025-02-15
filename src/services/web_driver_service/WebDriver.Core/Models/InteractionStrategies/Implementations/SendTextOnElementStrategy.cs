using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class SendTextOnElementStrategy : IInteractionStrategy<string>
{
    private readonly Guid _id;
    private readonly string _text;

    public SendTextOnElementStrategy(Guid id, string text)
    {
        _id = id;
        _text = text;
    }

    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> requested = instance.GetRunningDriver();
        if (requested.IsFailure)
            return await Task.FromResult(requested.Error);

        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return await Task.FromResult(element.Error);

        IWebDriver driver = requested.Value;
        IWebElement model = element.Value.Model;

        try
        {
            model.Clear();
            model.SendKeys(_text);
            return await Task.FromResult(_text);
        }
        catch (Exception ex)
        {
            return await Task.FromResult(
                new Error($"Can't write text in element: {_id}. Error: {ex.Message}")
            );
        }
    }
}
