using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public record GetSingleElementContract(string ElementPath, string ElementPathType) : IContract;
