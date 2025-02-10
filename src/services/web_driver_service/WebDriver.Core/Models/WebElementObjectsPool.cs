using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models;

internal sealed class WebElementObjectsPool
{
    private readonly Dictionary<Guid, WebElementObject> _objects = [];

    public Result RegisterObject(WebElementObject element)
    {
        return _objects.TryAdd(element.ElementId, element)
            ? Result.Success()
            : new Error("Can't register web element object in objects pool");
    }

    public void Refresh() => _objects.Clear();

    public Result<WebElementObject> this[Guid id]
    {
        get
        {
            bool contains = _objects.ContainsKey(id);
            if (!contains)
                return new Error($"Web element object with ID: {id} is not registered");
            return _objects[id];
        }
    }
}
