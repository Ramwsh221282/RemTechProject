using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace RemTechAvito.WebApi.Responses;

public class Envelope
{
    public int Code { get; set; }
    public object? Result { get; set; }
    public string Error { get; set; }
}

public static class EnvelopeExtensions
{
    public static IActionResult ToOkResult(this ControllerBase controller)
    {
        return controller.StatusCode(StatusCodes.Status204NoContent);
    }

    public static IActionResult ToOkResult<T>(this ControllerBase contoller, T result)
    {
        Envelope envelope = new Envelope();
        envelope.Code = (int)HttpStatusCode.OK;
        envelope.Error = "";
        envelope.Result = result;
        return contoller.Ok(envelope);
    }

    public static IActionResult ToErrorResult<T>(
        this ControllerBase controller,
        HttpStatusCode code,
        string errorMessage
    )
    {
        var envelope = new Envelope()
        {
            Code = (int)code,
            Result = null,
            Error = errorMessage,
        };
        return controller.StatusCode((int)code, envelope);
    }

    public static IActionResult ToErrorResult(
        this ControllerBase controller,
        HttpStatusCode code,
        string errorMessage
    )
    {
        var envelope = new Envelope()
        {
            Code = (int)code,
            Result = null,
            Error = errorMessage,
        };
        return controller.StatusCode((int)code, envelope);
    }
}
