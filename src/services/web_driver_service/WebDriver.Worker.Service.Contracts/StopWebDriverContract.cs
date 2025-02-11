using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record StopWebDriverContract : IContract;

public sealed record StopWebDriverContractResponse(bool IsStopped);
