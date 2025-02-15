using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class SendTextOnElementWithoutClearStrategy : IInteractionStrategy
{
    private readonly Guid _id;
    private readonly ReadOnlyMemory<char> _text;

    public SendTextOnElementWithoutClearStrategy(Guid id, ReadOnlyMemory<char> text)
    {
        _id = id;
        _text = text;
    }

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> requested = instance.GetRunningDriver();
        if (requested.IsFailure)
            return await Task.FromResult(requested.Error);

        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return await Task.FromResult(element.Error);

        IWebElement model = element.Value.Model;

        try
        {
            for (int index = 0; index < _text.Span.Length; index++)
                model.SendKeys(new string(_text.Span[index], 1));
            return await Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            return await Task.FromResult(
                new Error($"Can't write text in element: {_id}. Error: {ex.Message}")
            );
        }
    }
}
