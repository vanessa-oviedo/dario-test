using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("cases/{caseId:int}/status")]
public sealed class StatusController : ControllerBase
{
    private readonly IStatusService _status;
    public StatusController(IStatusService status) => _status = status;

    /// <summary>Cambia el estado del caso.</summary>
    [HttpPost]
    public async Task<IActionResult> Change([FromRoute] int caseId, [FromBody] ChangeCaseStatusDto body, CancellationToken ct)
    {
        var dto = new ChangeCaseStatusDto
        {
            CaseId = caseId,
            NewStatus = body.NewStatus,
            StatusDateUtc = body.StatusDateUtc,
            ChangedBy = string.IsNullOrWhiteSpace(body.ChangedBy) ? "api" : body.ChangedBy
        };
        await _status.ChangeStatusAsync(dto, ct);
        return NoContent();
    }
}
