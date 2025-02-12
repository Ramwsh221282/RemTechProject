using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts.BaseContracts;

public sealed record ScrollElementContract(Guid ExistingId) : IContract;
