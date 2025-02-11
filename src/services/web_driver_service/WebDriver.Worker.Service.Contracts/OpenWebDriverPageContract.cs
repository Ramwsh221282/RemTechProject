using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record OpenWebDriverPageContract(string Url) : IContract;
