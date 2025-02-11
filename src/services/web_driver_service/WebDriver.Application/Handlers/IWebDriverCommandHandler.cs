using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Application.Handlers;

public interface IWebDriverCommandHandler<in TCommand>
    where TCommand : IWebDriverCommand
{
    Task<Result> Handle(TCommand command);
}
