namespace RemTech.Parser.Contracts.Contracts;

public interface IWebDriverInstance
{
    bool IsDisposed { get; }

    void Instantiate();

    void Dispose();
}
