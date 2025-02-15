using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies.Implementations;

internal sealed class NewMultipleChildsStrategy
    : BaseSearchElementStrategy,
        IMultipleElementSearchStrategy
{
    private readonly Guid _existingId;
    private readonly string _path;
    private readonly string _type;

    public NewMultipleChildsStrategy(Guid existingId, string path, string type)
    {
        _existingId = existingId;
        _path = path;
        _type = type;
    }

    public async Task<Result<WebElementObject[]>> Search(WebDriverInstance instance)
    {
        Result<WebElementObject> existing = instance.GetFromPool(_existingId);
        if (existing.IsFailure)
            return await Task.FromResult(existing.Error);

        Result<By> search = Convert(_type, _path);
        if (search.IsFailure)
            return await Task.FromResult(search.Error);

        IReadOnlyCollection<IWebElement>? items;
        IWebElement model = existing.Value.Model;
        try
        {
            items = model.FindElements(search);
            WebElementObject[] result = items.Select(m => new WebElementObject(m)).ToArray();

            if (items.Count == 0)
                return await Task.FromResult(
                    new Error(
                        $"Can't find child elements (type: {_type} path: {_path}) of parent ({_existingId})"
                    )
                );

            foreach (var t in result)
                instance.AddInPool(t);

            return await Task.FromResult(result);
        }
        catch
        {
            return await Task.FromResult(
                new Error(
                    $"Can't find child elements (type: {_type} path: {_path}) of parent ({_existingId})"
                )
            );
        }
    }
}
