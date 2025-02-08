using RemTech.WebDriver.Plugin.Commands;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.WebDriver.Plugin.Contracts.CommandConverters;

internal interface ICommandConverter
{
    public Result<ICommand> Convert(WebDriverRequest? request);
}
