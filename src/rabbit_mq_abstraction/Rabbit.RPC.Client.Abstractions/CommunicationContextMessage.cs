namespace Rabbit.RPC.Client.Abstractions;

internal sealed class CommunicationContextMessage<TMessage>
{
    public TMessage Message { get; }
    public string CorellationId { get; }

    public CommunicationContextMessage(TMessage message)
    {
        Message = message;
        CorellationId = Guid.NewGuid().ToString();
    }
}
