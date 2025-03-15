using GuardValidationLibrary.GuardedFactory;
using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Dtos;
using RemTech.MainApi.ParsersManagement.Models;

namespace RemTech.MainApi.ParsersManagement.Messages.DataServiceMessages;

public sealed class ParserDataServiceMessager : IDisposable
{
    private readonly MultiCommunicationPublisher _publisher;

    public ParserDataServiceMessager(MultiCommunicationPublisher publisher) =>
        _publisher = publisher;

    public async Task<ContractActionResult> Send(ParserDataServiceMessage message) =>
        await _publisher.Send(message);

    public void Dispose() => _publisher.Dispose();
}

public abstract record ParserDataServiceMessage : IContract;

public sealed record SaveParserMessage(ParserDto Parser) : ParserDataServiceMessage;

public sealed record UpdateParserMessage(ParserDto Parser) : ParserDataServiceMessage;

public sealed record DeleteParserMessage(string Name) : ParserDataServiceMessage;

public sealed record GetAllParsersMessage : ParserDataServiceMessage;

public sealed record GetConcreteParserMessage(string Name) : ParserDataServiceMessage;

public sealed record ParserDaoResponse(
    string ParserName,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string State,
    string[] Links
)
{
    public Parser MapToParser()
    {
        ParserName name = new ParserName(ParserName);
        ParserSchedule schedule = new ParserSchedule(RepeatEveryHours, LastRun, NextRun);
        ParserState state = new ParserState(State);
        ParserLink[] links = Links.Select(l => new ParserLink(l)).ToArray();
        return GuardedCreator.Create<Parser>([name, schedule, state, links]).Object;
    }
}
