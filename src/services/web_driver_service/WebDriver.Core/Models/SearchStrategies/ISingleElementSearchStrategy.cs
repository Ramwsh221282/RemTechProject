using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies;

public interface ISingleElementSearchStrategy
{
    internal Task<Result<WebElementObject>> Search(WebDriverInstance instance);
}
