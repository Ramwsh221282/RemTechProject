using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record ScrollPageTopContract : IContract;

public sealed record ScrollPageTopResponse(bool IsScrolled);
