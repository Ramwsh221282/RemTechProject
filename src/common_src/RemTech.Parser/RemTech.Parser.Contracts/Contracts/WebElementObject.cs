namespace RemTech.Parser.Contracts.Contracts;

public record WebElementObject
{
    public string ElementPath { get; }
    public string ElementPathType { get; }
    public int Position { get; }

    protected WebElementObject(string path, string type, int position)
    {
        ElementPath = path;
        ElementPathType = type;
        Position = position;
    }
}
