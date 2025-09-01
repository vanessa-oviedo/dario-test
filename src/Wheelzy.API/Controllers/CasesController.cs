using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.API.Controllers;

[ApiController]
[Route("cases")]
public sealed class CasesController : ControllerBase
{
    private readonly IOrderService _cases;
    public CasesController(IOrderService cases) => _cases = cases;

    [HttpGet("search")]
    public async Task<ActionResult<PageResult<CaseSummaryDto>>> Search([FromQuery] SearchCasesRequest req, //TODO: Create a class at application level for the query parameters
        CancellationToken ct)
    {
        var page = await _cases.SearchSummariesAsync(req, ct);
        return Ok(page);
    }
}


