using Microsoft.AspNetCore.Mvc;
using RemTech.MainApi.CharacteristicsManagement.Features.GetCharacteristics;
using RemTech.MainApi.CharacteristicsManagement.Responses;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.Common.Envelope;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.CharacteristicsManagement.Endpoints;

[EndpointMapping]
public static class CharacteristicsEndpoint
{
    [EndpointMappingMethod]
    public static void Map(this WebApplication app)
    {
        var group = app.MapGroup("api/characteristics").RequireCors("frontend");
        group.MapGet("", GetCharacteristics);
    }

    private static async Task<IResult> GetCharacteristics(
        [FromServices]
            IRequestHandler<GetCharacteristicsRequest, Option<CharacteristicResponse[]>> handler,
        CancellationToken ct
    )
    {
        GetCharacteristicsRequest request = new GetCharacteristicsRequest();
        Option<CharacteristicResponse[]> response = await handler.Handle(request, ct);
        return response.HasValue
            ? EnvelopeResultExtensions.Envelope<CharacteristicResponse[]>(response.Value)
            : EnvelopeResultExtensions.NotFound();
    }
}
