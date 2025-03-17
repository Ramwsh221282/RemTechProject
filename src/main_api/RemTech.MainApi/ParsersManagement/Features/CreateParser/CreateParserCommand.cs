using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Models;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

public sealed record CreateParserCommand(string ParserName) : ICommand<Parser>;
