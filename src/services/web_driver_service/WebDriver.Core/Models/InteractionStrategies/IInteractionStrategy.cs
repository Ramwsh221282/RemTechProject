using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.InteractionStrategies;

public interface IInteractionStrategy
{
    internal Task<Result> Perform(WebDriverInstance instance);
}

public interface IInteractionStrategy<T>
{
    internal Task<Result<T>> Perform(WebDriverInstance instance);
}
