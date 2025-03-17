using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

public sealed class CreateParserContext
{
    public Option<Parser> Parser { get; set; } = Option<Parser>.None();
    public Option<Error> Error { get; set; } = Option<Error>.None();
}
