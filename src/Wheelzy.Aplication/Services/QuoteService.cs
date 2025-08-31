using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Services;

public sealed class QuoteService : IQuoteService
{
    private readonly ICaseRepository _cases;
    private readonly IRateRepository _rates;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public QuoteService(ICaseRepository cases, IRateRepository rates, IUnitOfWork uow, IClock clock)
    {
        _cases = cases;
        _rates = rates;
        _uow   = uow;
        _clock = clock;
    }

    public async Task<int> GenerateBaseQuotesAsync(GenerateBaseQuotesDto dto, CancellationToken ct = default)
    {
        var agg = await _cases.GetByIdAsync(dto.CaseId, includeRelated: true, ct)
                  ?? throw new InvalidOperationException("Case no encontrado.");

        var rates = await _rates.GetBaseRatesByZipAsync(agg.ZipCode, ct);
        if (rates.Count == 0) return 0;

        var now = _clock.UtcNow;

        foreach (var (buyerId, amount) in rates)
        {
            agg.Quotes.Add(new CaseQuote
            {
                BuyerId      = buyerId,
                Amount       = amount,
                IsCurrent    = false,
                CreatedAtUtc = now,
                CaseId       = agg.Id
            });
        }

        if (dto.SetBestAsCurrent)
        {
            var best = agg.Quotes.OrderByDescending(q => q.Amount).FirstOrDefault();
            if (best is not null)
            {
                foreach (var q in agg.Quotes) q.IsCurrent = false;
                best.IsCurrent = true;
            }
        }

        await _cases.UpdateAsync(agg, ct);
        return await _uow.SaveChangesAsync(ct);
    }

    public async Task SetCurrentQuoteAsync(SetCurrentQuoteDto dto, CancellationToken ct = default)
    {
        var agg = await _cases.GetByIdAsync(dto.CaseId, includeRelated: true, ct)
                  ?? throw new InvalidOperationException("Case no encontrado.");

        var quote = agg.Quotes.LastOrDefault(q => q.BuyerId == dto.BuyerId)
                    ?? throw new InvalidOperationException("Quote no encontrada para ese Buyer.");

        foreach (var q in agg.Quotes) q.IsCurrent = false;
        quote.IsCurrent = true;

        await _cases.UpdateAsync(agg, ct);
        await _uow.SaveChangesAsync(ct);
    }
}
