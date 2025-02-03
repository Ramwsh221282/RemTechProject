namespace RemTech.Parser.Contracts.Contracts;

public enum ElementSelectorType
{
    Xpath,
    Classname,
}

public abstract record TryGetElementRequest
{
    protected readonly string _elementSelector;
    protected readonly ElementSelectorType _type;

    protected TryGetElementRequest(string selector, ElementSelectorType type)
    {
        _elementSelector = selector;
        _type = type;
    }
}
