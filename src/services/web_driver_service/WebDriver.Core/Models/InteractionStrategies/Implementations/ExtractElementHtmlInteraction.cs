using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies.Implementations;

internal sealed class ExtractElementHtmlInteraction : IInteractionStrategy<string>
{
    private readonly Guid _id;

    public ExtractElementHtmlInteraction(Guid id) => _id = id;

    public async Task<Result<string>> Perform(WebDriverInstance instance)
    {
        Result<WebElementObject> element = instance.GetFromPool(_id);
        if (element.IsFailure)
            return await Task.FromResult(element.Error);

        IWebElement model = element.Value.Model;

        string html = model.GetAttribute("outerHTML");
        element.Value.SetOuterHtml(html);
        return await Task.FromResult(html);
    }
}
