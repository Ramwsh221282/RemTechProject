using Rabbit.RPC.Client.Abstractions;

namespace WebDriver.Worker.Service.Contracts;

public sealed record GetPageHtmlContract : IContract;

public sealed record GetHtmlResponse(string Html);
