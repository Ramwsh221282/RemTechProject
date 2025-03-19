using Rabbit.RPC.Client.Abstractions;

namespace SharedParsersLibrary.ParserMessaging;

internal sealed record UpdateParserMessage(ParserDto Parser) : IContract;
