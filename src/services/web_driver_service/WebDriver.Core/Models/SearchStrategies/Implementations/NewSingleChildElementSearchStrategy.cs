using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies.Implementations;

internal sealed class NewSingleChildElementSearchStrategy
    : BaseSearchElementStrategy,
        ISingleElementSearchStrategy
{
    private readonly Guid _existingId;
    private readonly string _type;
    private readonly string _path;

    public NewSingleChildElementSearchStrategy(Guid existingId, string type, string path)
    {
        _existingId = existingId;
        _type = type;
        _path = path;
    }

    public async Task<Result<WebElementObject>> Search(WebDriverInstance instance)
    {
        Result<WebElementObject> existingElement = instance.GetFromPool(_existingId);
        if (existingElement.IsFailure)
            return await Task.FromResult(existingElement.Error);

        Result<By> search = Convert(_type, _path);
        if (search.IsFailure)
            return await Task.FromResult(search.Error);

        IWebElement model = existingElement.Value.Model;
        try
        {
            WebElementObject child = new(model.FindElement(search));
            instance.AddInPool(child);
            return await Task.FromResult(child);
        }
        catch
        {
            return await Task.FromResult(
                new Error($"Can't find child of {_existingId} with path: {_path} and type: {_type}")
            );
        }
    }
}
