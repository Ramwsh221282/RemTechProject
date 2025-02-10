using OpenQA.Selenium;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Core.Models.SearchStrategies;

internal sealed class ElementSearchTypeConverter
{
    public Result<By> Convert(string type, string path)
    {
        if (string.IsNullOrWhiteSpace(type))
            return new Error("Type should be specified");
        return type switch
        {
            "xpath" => By.XPath(path),
            "tag" => By.TagName(path),
            "class" => By.ClassName(path),
            _ => new Error("Path type is not supported"),
        };
    }
}
