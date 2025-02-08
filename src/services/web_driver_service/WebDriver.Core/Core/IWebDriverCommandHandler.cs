using RemTech.WebDriver.Plugin.Commands;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Core;

internal interface IWebDriverCommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command);
}
