using OpenQA.Selenium;

namespace WebDriver.Core.Models;

public record WebElementObject
{
    public string ElementPath { get; }
    public string ElementPathType { get; }
    public Guid ElementId { get; }
    public IWebElement Model { get; }

    internal WebElementObject(string path, string type, IWebElement model)
    {
        ElementPath = path;
        ElementPathType = type;
        ElementId = Guid.NewGuid();
        Model = model;
    }
}
