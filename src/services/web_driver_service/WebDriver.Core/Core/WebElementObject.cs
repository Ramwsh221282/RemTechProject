using OpenQA.Selenium;

namespace WebDriver.Core.Core;

public record WebElementObject
{
    public string ElementPath { get; }
    public string ElementPathType { get; }
    public Guid ElementId { get; }
    public IWebElement Model { get; }

    public WebElementObject(string path, string type, IWebElement model)
    {
        ElementPath = path;
        ElementPathType = type;
        ElementId = Guid.NewGuid();
        Model = model;
    }
}
