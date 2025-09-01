using Wheelzy.Application.DTOs.Case;
using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Services;

public sealed class QuoteService : IQuoteService
{
    private readonly IOrderRepository _orders;
    private readonly IRateRepository _rates;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IBuyerZipCoverageReadRepository _coverage;
    private readonly IOrderBuyerQuoteWriteRepository _quotes;
    private readonly IOrderBuyerQuoteReadRepository _quotesRead;

    public QuoteService(IOrderRepository orders, IRateRepository rates, IUnitOfWork uow, IClock clock, IOrderBuyerQuoteWriteRepository quotes, IBuyerZipCoverageReadRepository coverage, IOrderBuyerQuoteReadRepository quotesRead)
    {
        _orders = orders;
        _rates = rates;
        _uow   = uow;
        _clock = clock;
        _orders = orders;
        _coverage = coverage;
        _quotes = quotes;
        _quotesRead = quotesRead;
    }
    

    public async Task<int> GenerateBaseQuotesAsync(CreateQuoteCommand cmd, CancellationToken ct = default)
    {
        var now = cmd.NowUtc ?? DateTime.UtcNow;
        var newQuoteId = 0;
        decimal amountUsed = 0m;

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            // 1) ZIP real de la order (fuente de verdad)
            var orderZip = await _orders.GetOrderZipAsync(cmd.OrderId, ct);

            // 2) Validar cobertura Buyer × ZIP (trae BuyerZipCoverageId y default)
            var cov = await _coverage.GetCoverageAsync(cmd.BuyerId, orderZip, ct);
            if (cov is null)
                throw new InvalidOperationException($"Buyer {cmd.BuyerId} cannot quote on ZIP {orderZip}.");

            var (buyerZipCoverageId, defaultAmount) = cov.Value;

            // 3) Determinar monto (override o default de cobertura)
            amountUsed = cmd.AmountOverride ?? defaultAmount;
            if (amountUsed <= 0)
                throw new ArgumentOutOfRangeException(nameof(cmd.AmountOverride), "Amount must be > 0.");

            // 4) Insertar la quote (NO marcar current acá)
            newQuoteId = await _quotes.AddAsync(cmd.OrderId, buyerZipCoverageId, amountUsed, now, ct);

            await _uow.SaveChangesAsync(ct);
        }, ct);

        return newQuoteId; 
    }

    public async Task SetCurrentQuoteAsync(long orderId, long? orderBuyerQuoteId, CancellationToken t = default)
    {
        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            if (orderBuyerQuoteId.HasValue)
            {
                var exists = await _quotesRead.ExistsForOrderAsync(orderId, orderBuyerQuoteId.Value, t);
                if (!exists)
                    throw new InvalidOperationException(
                        $"Quote {orderBuyerQuoteId.Value} does not belong to Order {orderId}.");
            }

            await _orders.SetCurrentBuyerQuoteAsync(orderId, orderBuyerQuoteId, t);
            await _uow.SaveChangesAsync(t);
        }, t);
    }
}
