using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverExecutableManager
{
    public Result<string> Install();
    public Result<string> Uninstall();
}
