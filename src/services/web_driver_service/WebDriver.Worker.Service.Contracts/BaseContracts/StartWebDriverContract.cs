using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public record StartWebDriverContract(string LoadStrategy) : IContract;
