using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("cases/{orderId:int}/status")]
public sealed class StatusController : ControllerBase
{
    private readonly IStatusService _status;
    public StatusController(IStatusService status) => _status = status;

    /// <summary>Cambia el estado del caso.</summary>
    [HttpPost]
    public async Task<IActionResult> Change([FromRoute] int orderId, [FromBody] ChangeCaseStatusDto body, CancellationToken ct)
    {
        await _status.UpdateStatusAsync(orderId, (int)body.NewStatus, body.StatusDateUtc, "system",  ct);
        return NoContent();
    }
}
