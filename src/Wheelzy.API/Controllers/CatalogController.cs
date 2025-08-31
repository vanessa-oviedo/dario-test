using Microsoft.AspNetCore.Mvc;
using Wheelzy.Application.Interfaces.Repositories;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("catalog")]
public sealed class CatalogController : ControllerBase
{
    private readonly ICatalogRepository _catalog;
    public CatalogController(ICatalogRepository catalog) => _catalog = catalog;

    [HttpGet("makes")]
    public async Task<IActionResult> GetMakes(CancellationToken ct)
        => Ok((await _catalog.ListMakesAsync(ct)).Select(x => new { x.Id, x.Name }));

    [HttpGet("models")]
    public async Task<IActionResult> GetModels([FromQuery] int makeId, CancellationToken ct)
        => Ok((await _catalog.ListModelsByMakeAsync(makeId, ct)).Select(x => new { x.Id, x.MakeId, x.Name }));

    [HttpGet("submodels")]
    public async Task<IActionResult> GetSubModels([FromQuery] int modelId, CancellationToken ct)
        => Ok((await _catalog.ListSubModelsByModelAsync(modelId, ct)).Select(x => new { x.Id, x.ModelId, x.Name }));
}