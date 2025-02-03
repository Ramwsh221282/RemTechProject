using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverFactory
{
    Result<IWebDriverInstance> Create();
}
