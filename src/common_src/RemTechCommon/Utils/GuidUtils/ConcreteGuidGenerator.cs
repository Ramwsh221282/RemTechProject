namespace RemTechCommon;

public sealed class ConcreteGuidGenerator : IGuidGenerationStrategy
{
    private readonly Guid _id;

    public ConcreteGuidGenerator(Guid id) => _id = id;

    public Guid Generate() => _id;
}
