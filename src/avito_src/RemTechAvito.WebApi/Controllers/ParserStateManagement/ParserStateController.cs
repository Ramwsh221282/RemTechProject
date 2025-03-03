using System.Net;
using Microsoft.AspNetCore.Mvc;
using RemTechAvito.WebApi.BackgroundServices;
using RemTechAvito.WebApi.Responses;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.WebApi.Controllers.ParserStateManagement;

public sealed record ParserServiceState(bool IsEnabled);

public sealed class ParserStateController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetParserState([FromServices] ParserBackgroundService service)
    {
        var state = new ParserServiceState(service.IsWorking);
        return this.ToOkResult(state);
    }

    [HttpPost("enable")]
    public async Task<IActionResult> EnableParserService(
        [FromServices] ParserBackgroundService service
    )
    {
        if (service.IsWorking)
        {
            var error = new Error("Сервис парсера уже запущен");
            return this.ToErrorResult(HttpStatusCode.BadRequest, error.Description);
        }

        service.StartAsync(new CancellationTokenSource().Token);
        return this.ToOkResult();
    }

    [HttpPost("disable")]
    public async Task<IActionResult> DisableParserService(
        [FromServices] ParserBackgroundService service,
        CancellationToken ct = default
    )
    {
        if (!service.IsWorking)
        {
            var error = new Error("Сервис уже остановлен");
            return this.ToErrorResult(HttpStatusCode.BadRequest, error.Description);
        }

        service.StopAsync(new CancellationTokenSource().Token);
        return this.ToOkResult();
    }

    [HttpPost("restart")]
    public async Task<IActionResult> RestartService(
        [FromServices] ParserBackgroundService service,
        CancellationToken ct = default
    )
    {
        if (service.IsWorking)
        {
            service.StopAsync(new CancellationTokenSource().Token);
            service.StartAsync(new CancellationTokenSource().Token);
        }
        else
        {
            service.StartAsync(new CancellationTokenSource().Token);
        }

        return this.ToOkResult();
    }
}
