namespace RemTech.WebDriver.Plugin.Core;

internal record WebElementObject
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
