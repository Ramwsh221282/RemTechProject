using GuardValidationLibrary.GuardedFactory;
using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Models;

namespace RemTech.MainApi.ParsersManagement.Messages;

public sealed class DataServiceMessager
{
    private readonly IMessagePublisher _publisher;

    public DataServiceMessager(IMessagePublisher publisher) => _publisher = publisher;

    public async Task<ContractActionResult> Send<TMessage>(
        TMessage message,
        CancellationToken ct = default
    )
        where TMessage : IContract
    {
        using (_publisher)
        {
            return await _publisher.Send(message, ct);
        }
    }
}

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
        return GuardedCreator.Create<Parser>(name, schedule, state, links).Object;
    }
}
