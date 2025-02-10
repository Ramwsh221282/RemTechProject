using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies.Implementations;

internal sealed class NewSingleElementSearchStrategy
    : BaseSearchElementStrategy,
        ISingleElementSearchStrategy
{
    private readonly string _type;
    private readonly string _path;

    public NewSingleElementSearchStrategy(string type, string path)
    {
        _type = type;
        _path = path;
    }

    public async Task<Result<WebElementObject>> Search(WebDriverInstance instance)
    {
        Result<IWebDriver> driver = instance.GetRunningDriver();
        if (driver.IsFailure)
            return await Task.FromResult(driver.Error);

        Result<By> search = Convert(_type, _path);
        if (search.IsFailure)
            return await Task.FromResult(search.Error);

        int attempts = 0;
        while (ReachedMaxAttempts(ref attempts))
        {
            try
            {
                WebElementObject element = new(_path, _type, driver.Value.FindElement(search));
                Result registration = instance.AddInPool(element);
                return registration.IsSuccess
                    ? await Task.FromResult(element)
                    : await Task.FromResult(registration.Error);
            }
            catch
            {
                await Wait();
                attempts++;
            }
        }

        return await Task.FromResult(
            new Error($"Cant find element with path: {_path} and type: {_type}")
        );
    }
}
