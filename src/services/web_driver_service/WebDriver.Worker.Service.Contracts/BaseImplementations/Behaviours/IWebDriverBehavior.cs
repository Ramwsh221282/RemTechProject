using Rabbit.RPC.Client.Abstractions;
using RemTechCommon.Utils.ResultPattern;

namespace WebDriver.Worker.Service.Contracts.BaseImplementations.Behaviours;

public interface IWebDriverBehavior
{
    Task<Result> Execute(IMessagePublisher publisher, CancellationToken ct = default);
}

public static class ContractActionResultExtensions
{
    public static Result ToResult(this ContractActionResult result) =>
        result.IsSuccess ? Result.Success() : Result.Failure(new Error(result.Error));
}
