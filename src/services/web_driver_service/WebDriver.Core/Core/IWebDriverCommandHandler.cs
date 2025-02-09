using RemTechCommon.Utils.ResultPattern;
using WebDriver.Core.Commands;

namespace WebDriver.Core.Core;

public interface IWebDriverCommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command);
}
