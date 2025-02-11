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

    public static IInteractionStrategy Click(Guid elementId) =>
        new ClickOnElementStrategy(elementId);
}
