using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public record StartWebDriverContract(string LoadStrategy) : IContract;

public record StartWebDriverContractResponse(bool IsStarted);
