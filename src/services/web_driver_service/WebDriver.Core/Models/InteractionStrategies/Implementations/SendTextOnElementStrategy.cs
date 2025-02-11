using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class SendTextOnElementStrategy : IInteractionStrategy<string>
{
    private readonly Guid _id;
    private readonly string _text;
    private readonly ILogger _logger;

    public SendTextOnElementStrategy(Guid id, string text, ILogger logger)
    {
        _id = id;
        _text = text;
        _logger = logger;
    }

    public async Task<Result<string>> Perform(WebDriverInstance instance)
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
            model.Clear();
            model.SendKeys(_text);
            return await Task.FromResult(_text);
        }
        catch (Exception ex)
        {
            _logger.Error("Error in writing text in element. Exception: {Ex}", ex.Message);
            return new Error($"Can't write text in element: {_id}");
        }
    }
}
