using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

public sealed record CreateParserCommand(string ParserName) : IRequest<Result<Parser>>;
