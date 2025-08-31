using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("cases/{caseId:int}/quotes")]
public sealed class QuotesController : ControllerBase
{
    private readonly IQuoteService _quotes;
    public QuotesController(IQuoteService quotes) => _quotes = quotes;

    /// <summary>Genera cotizaciones base por ZIP para el caso.</summary>
    [HttpPost("base")]
    public async Task<ActionResult<object>> GenerateBase([FromRoute] int caseId, [FromBody] GenerateBaseQuotesDto body, CancellationToken ct)
    {
        var dto = new GenerateBaseQuotesDto { CaseId = caseId, SetBestAsCurrent = body.SetBestAsCurrent };
        var inserted = await _quotes.GenerateBaseQuotesAsync(dto, ct);
        return Ok(new { inserted });
    }

    /// <summary>Setea la cotización actual.</summary>
    [HttpPost("current")]
    public async Task<IActionResult> SetCurrent([FromRoute] int caseId, [FromBody] SetCurrentQuoteDto body, CancellationToken ct)
    {
        var dto = new SetCurrentQuoteDto { CaseId = caseId, BuyerId = body.BuyerId, WhenUtc = body.WhenUtc };
        await _quotes.SetCurrentQuoteAsync(dto, ct);
        return NoContent();
    }
}
