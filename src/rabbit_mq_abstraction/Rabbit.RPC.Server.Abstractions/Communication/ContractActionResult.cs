namespace Rabbit.RPC.Server.Abstractions.Communication;

public sealed class ContractActionResult
{
    public string Error { get; }
    public bool IsSuccess { get; }
    public object? Body { get; }

    public ContractActionResult(string error)
    {
        Error = error;
        IsSuccess = false;
    }

    public ContractActionResult(object body)
    {
        IsSuccess = true;
        Body = body;
        Error = String.Empty;
    }
}
