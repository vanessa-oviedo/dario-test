using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wheelzy.API.Requests;
using Wheelzy.Application.DTOs.Common;
using Wheelzy.Application.DTOs.Order;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.API.Controllers;

[ApiController]
[Route("order")]
public sealed class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IMapper _mapper;
    private readonly IStatusService _statusService;

    public OrderController(IOrderService orderService, IStatusService statusService, IMapper mapper)
    {
        _orderService = orderService;
        _mapper = mapper;
        _statusService = statusService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<PageResult<OrderSummaryDto>>> Search([FromQuery] Requests.SearchOrdersRequest req,
        CancellationToken ct)
    {
        var page = await _orderService.SearchSummaries(_mapper.Map<Application.Requests.SearchOrdersRequest>(req), ct);
        return Ok(page);
    }

    [HttpPost("set-status")]
    public async Task<IActionResult> SetStatus([FromBody] ChangeOrderStatusRequest request, CancellationToken ct)
    {
        await _statusService.UpdateStatus(request.OrderId.Value, (int)request.NewStatus, request.StatusDateUtc, ct);
        return NoContent();
    }
}


