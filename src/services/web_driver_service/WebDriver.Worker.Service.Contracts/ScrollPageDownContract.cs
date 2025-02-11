using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public record ScrollPageDownContract : IContract;

public sealed record ScrollPageDownContractResponse(bool IsScrolled);
