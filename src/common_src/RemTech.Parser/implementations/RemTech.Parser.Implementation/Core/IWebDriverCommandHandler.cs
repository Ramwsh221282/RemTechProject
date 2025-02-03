using RemTech.Parser.Contracts.Contracts;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Parser.Implementation.Core;

public interface IWebDriverCommandHandler<in TCommand>
    where TCommand : IWebDriverCommand
{
    Task<Result> Handle(TCommand command);
}
