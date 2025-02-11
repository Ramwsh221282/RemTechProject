namespace Rabbit.RPC.Server.Abstractions.Communication;

public sealed record ContractActionResult(string Error, bool IsSuccess, object? Body)
{
    public static ContractActionResult Fail(string error) =>
        new ContractActionResult(error, false, null);

    public static ContractActionResult Success(object body) =>
        new ContractActionResult(string.Empty, true, body);
}
