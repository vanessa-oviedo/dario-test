using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.Interfaces.Repositories;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("buyers")]
public sealed class BuyersController : ControllerBase
{
    private readonly IBuyerRepository _buyers;
    public BuyersController(IBuyerRepository buyers) => _buyers = buyers;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok((await _buyers.GetAllAsync(ct)).Select(b => new { b.Id, b.Name }));
}