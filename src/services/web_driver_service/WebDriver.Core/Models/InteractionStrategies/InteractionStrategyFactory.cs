using Serilog;
using WebDriver.Core.Models.InteractionStrategies.Implementations;

namespace WebDriver.Core.Models.InteractionStrategies;

public static class InteractionStrategyFactory
{
    public static IInteractionStrategy CreateScrollTop() => new ScrollToTopInteraction();

    public static IInteractionStrategy CreateScrollToBottom() => new ScrollToBottomInteraction();

    public static IInteractionStrategy<string> CreateOpenPage(string webPageUrl) =>
        new OpenPageInteraction(webPageUrl);

    public static IInteractionStrategy<string> ExtractText(Guid elementId) =>
        new ExtractTextInteraction(elementId);

    public static IInteractionStrategy<string> ExtractAttribute(Guid elementId, string attribute) =>
        new GetElementAttributeInteraction(elementId, attribute);

    public static IInteractionStrategy<string> ExtractHtml() => new ExtractHtmlInteraction();

    public static IInteractionStrategy<string> ExtractHtml(Guid elementId) =>
        new ExtractElementHtmlInteraction(elementId);

    public static IInteractionStrategy<string> SendText(Guid elementId, string text) =>
        new SendTextOnElementStrategy(elementId, text);

    public static IInteractionStrategy Click(Guid elementId) =>
        new ClickOnElementStrategy(elementId);

    public static IInteractionStrategy ScrollElement(Guid elementId) =>
        new ScrollElementStrategy(elementId);

    public static IInteractionStrategy SendTextNoClear(Guid elementId, ReadOnlyMemory<char> text) =>
        new SendTextOnElementWithoutClearStrategy(elementId, text);

    public static IInteractionStrategy ClearText(Guid elementId) =>
        new ClearTextStrategy(elementId);
}
