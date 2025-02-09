using System.Text.Json;

namespace Rabbit.RPC.Client.Abstractions;

public sealed record ContractActionResult(string Error, bool IsSuccess, JsonElement Body);

public static class ContractActionResultExtensions
{
    public static TResponse FromResult<TResponse>(this ContractActionResult result)
    {
        if (!result.IsSuccess)
            throw new InvalidCastException("Cannot access body of failure result");
        TResponse response = JsonSerializer.Deserialize<TResponse>(result.Body)!;
        return response;
    }
}
