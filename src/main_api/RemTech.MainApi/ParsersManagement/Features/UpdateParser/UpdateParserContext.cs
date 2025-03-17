using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser;

public sealed class UpdateParserContext
{
    public Option<Error> Error { get; set; } = Option<Error>.None();
    public Option<Parser> UpdatedModel { get; set; } = Option<Parser>.None();
}
