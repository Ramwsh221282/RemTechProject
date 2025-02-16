using OpenQA.Selenium;

namespace WebDriver.Core.Models;

public record WebElementObject
{
    public IWebElement Model { get; }
    public Guid ElementId { get; }

    internal WebElementObject(IWebElement model)
    {
        Model = model;
        ElementId = Guid.NewGuid();
    }
}

public record WebElementResponseObject(Guid Id, string OuterHTML, string InnerText);
