using RemTechCommon;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverCommand
{
    Task<Result> Execute();
}
