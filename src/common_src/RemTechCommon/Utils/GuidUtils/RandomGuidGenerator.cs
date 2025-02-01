namespace RemTechCommon;

public sealed class RandomGuidGenerator : IGuidGenerationStrategy
{
    public Guid Generate() => Guid.NewGuid();
}
