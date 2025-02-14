using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ClearTextStrategy : IInteractionStrategy
{
    private readonly Guid _id;

    public ClearTextStrategy(Guid id)
    {
        _id = id;
    }

    public async Task<Result> Perform(WebDriverInstance instance)
    {
        Result<IWebDriver> request = instance.GetRunningDriver();
        if (request.IsFailure)
            return await Task.FromResult(request);

        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return await Task.FromResult(element);

        IWebElement model = element.Value.Model;

        try
        {
            model.Clear();
            return await Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            return await Task.FromResult(
                new Error($"Error occured during text clearing: {ex.Message}")
            );
        }
    }
}
