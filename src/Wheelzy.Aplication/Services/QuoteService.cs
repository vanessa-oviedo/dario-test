using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Requests;

namespace Wheelzy.Application.Services;

public sealed class QuoteService : IQuoteService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IBuyerZipCoverageReadRepository _coverageRepository;
    private readonly IOrderBuyerQuoteWriteRepository _quotesRepository;
    private readonly IOrderBuyerQuoteReadRepository _quotesReadRepository;

    public QuoteService(IOrderRepository orderRepository, 
        IUnitOfWork uow, IClock clock, 
        IOrderBuyerQuoteWriteRepository quotesRepository, 
        IBuyerZipCoverageReadRepository coverageRepository, 
        IOrderBuyerQuoteReadRepository quotesReadRepository)
    {
        _orderRepository = orderRepository;
        _uow   = uow;
        _clock = clock;
        _coverageRepository = coverageRepository;
        _quotesRepository = quotesRepository;
        _quotesReadRepository = quotesReadRepository;
    }
    

    public async Task<int> GenerateBaseQuotes(CreateQuoteRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var newQuoteId = 0;
        decimal amountUsed = 0m;

        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            var orderZip = await _orderRepository.GetOrderZip(request.OrderId, ct);

            var cov = await _coverageRepository.GetCoverage(request.BuyerId, orderZip, ct);
            if (cov is null)
                throw new InvalidOperationException($"Buyer {request.BuyerId} cannot quote on ZIP {orderZip}.");

            var (buyerZipCoverageId, defaultAmount) = cov.Value;

            amountUsed = request.AmountOverride ?? defaultAmount;
            if (amountUsed <= 0)
                throw new ArgumentOutOfRangeException(nameof(request.AmountOverride), "Amount must be > 0.");

            newQuoteId = await _quotesRepository.Add(request.OrderId, buyerZipCoverageId, amountUsed, now, ct);

            await _uow.SaveChanges(ct);
        }, ct);

        return newQuoteId; 
    }

    public async Task SetCurrentQuote(int orderId, int? orderBuyerQuoteId, CancellationToken t = default)
    {
        await _uow.ExecuteInTransactionAsync(async (_) =>
        {
            if (orderBuyerQuoteId.HasValue)
            {
                var exists = await _quotesReadRepository.ExistsForOrderAsync(orderId, orderBuyerQuoteId.Value, t);
                if (!exists)
                    throw new InvalidOperationException(
                        $"Quote {orderBuyerQuoteId.Value} does not belong to Order {orderId}.");
            }

            await _orderRepository.SetCurrentBuyerQuote(orderId, orderBuyerQuoteId, t);
            await _uow.SaveChanges(t);
        }, t);
    }
}
