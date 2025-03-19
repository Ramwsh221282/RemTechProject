using Rabbit.RPC.Client.Abstractions;

namespace SharedParsersLibrary.ParserMessaging;

internal sealed record GetParserMessage(ParserQueryPayload Payload) : IContract;
