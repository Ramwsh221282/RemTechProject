using Microsoft.AspNetCore.Mvc;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechAvito.WebApi.Responses;

namespace RemTechAvito.WebApi.Controllers.ParserJournalsManagement;

public sealed class ParserJournalsController : ApplicationController
{
    [HttpGet("parser-journals")]
    public async Task<IActionResult> Get(
        [FromServices] IParserJournalQueryRepository repository,
        [FromQuery] int page,
        [FromQuery] int size,
        CancellationToken ct = default
    )
    {
        var journals = await repository.Get(page, size, ct);
        return this.ToOkResult(journals);
    }
}
