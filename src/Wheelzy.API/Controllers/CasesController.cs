using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.API.Controllers;

[ApiController]
[Route("cases")]
public sealed class CasesController : ControllerBase
{
    private readonly ICaseService _cases;
    public CasesController(ICaseService cases) => _cases = cases;

    /// <summary>Crea un caso de venta.</summary>
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateSellCaseDto dto, CancellationToken ct)
    {
        var id = await _cases.CreateSellCaseAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { caseId = id }, new { id });
    }

    /// <summary>Detalle del caso.</summary>
    [HttpGet("{caseId:int}")]
    public async Task<ActionResult<SellCaseDetailDto>> GetById([FromRoute] int caseId, CancellationToken ct)
    {
        var detail = await _cases.GetDetailAsync(caseId, ct);
        return detail is null ? NotFound() : Ok(detail);
    }

    /// <summary>Busca casos paginados.</summary>
    [HttpPost("search")]
    public async Task<ActionResult<PageResult<CaseSummaryDto>>> Search([FromBody] SearchCasesRequest req,
        CancellationToken ct)
    {
        var page = await _cases.SearchSummariesAsync(req, ct);
        return Ok(page);
    }
}


