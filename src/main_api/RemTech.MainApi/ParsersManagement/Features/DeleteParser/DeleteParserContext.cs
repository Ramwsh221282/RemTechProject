using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser;

public sealed class DeleteParserContext
{
    public Option<ParserQueryPayload> Payload { get; set; } = Option<ParserQueryPayload>.None();
}
