using Rabbit.RPC.Server.Abstractions.Communication;

namespace RemTech.MongoDb.Service.Extensions;

public static class ContractActionResultExtensions
{
    public static ContractActionResult Success<T>(this T data) =>
        ContractActionResult.Success(data!);
}
