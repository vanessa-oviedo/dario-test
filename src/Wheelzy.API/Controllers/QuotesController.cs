using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Wheelzy.API.Requests;
using Wheelzy.Application.Interfaces.Service;

namespace Wheelzy.Api.Controllers;

[ApiController]
[Route("orders/quotes")]
public sealed class QuotesController : ControllerBase
{
    private readonly IQuoteService _quotesService;
    private readonly IMapper _mapper;

    public QuotesController(IQuoteService quotesService, IMapper mapper)
    {
        _quotesService = quotesService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult<int>> GenerateBase([FromBody] CreateQuoteRequest body, CancellationToken ct) //TODO: DONT USE OBJECT
    {
        var inserted = await _quotesService.GenerateBaseQuotes(_mapper.Map<Wheelzy.Application.Requests.CreateQuoteRequest>(body), ct);
        return Ok(inserted);
    }

    /// <summary>Setea la cotización actual.</summary>
    //[HttpPost("current")]
    //public async Task<IActionResult> SetCurrent([FromRoute] int caseId, [FromBody] SetCurrentQuoteDto body, CancellationToken ct)
    //{
    //    var dto = new SetCurrentQuoteDto { CaseId = caseId, BuyerId = body.BuyerId, WhenUtc = body.WhenUtc };
    //    await _quotes.SetCurrentQuoteAsync(dto, ct);
    //    return NoContent();
    //}
}
