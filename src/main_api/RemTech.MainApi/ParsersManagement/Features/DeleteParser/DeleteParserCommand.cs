using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser;

public record DeleteParserCommand(string ParserName) : IRequest<Result>;
