using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies;

public interface IMultipleElementSearchStrategy
{
    internal Task<Result<WebElementObject[]>> Search(WebDriverInstance instance);
}
