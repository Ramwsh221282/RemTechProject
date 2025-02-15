using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models;

internal sealed class WebElementObjectsPool
{
    private readonly Dictionary<Guid, WebElementObject> _objects = [];

    public void RegisterObject(WebElementObject element) =>
        _objects.Add(element.ElementId, element);

    public void Refresh()
    {
        foreach (var element in _objects)
            element.Value.Dispose();
        _objects.Clear();
    }

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
