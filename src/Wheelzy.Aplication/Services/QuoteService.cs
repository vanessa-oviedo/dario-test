using Wheelzy.Application.Interfaces;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Service;
using Wheelzy.Application.Requests;
using Wheelzy.Application.Strategy;

namespace Wheelzy.Application.Services;

public sealed class QuoteService : IQuoteService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;
    private readonly IBuyerZipCoverageRepository _coverageRepository;
    private readonly IOrderBuyerQuoteRepository _orderBuyerQuoteRepository;

    public QuoteService(IOrderRepository orderRepository,
        IUnitOfWork uow, IClock clock,
        IBuyerZipCoverageRepository coverageRepository,
        IOrderBuyerQuoteRepository orderBuyerQuoteRepository)
    {
        _orderRepository = orderRepository;
        _uow = uow;
        _clock = clock;
        _coverageRepository = coverageRepository;
        _orderBuyerQuoteRepository = orderBuyerQuoteRepository;
    }


    public async Task<int> GenerateBaseQuotes(CreateQuoteRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var newQuoteId = 0;
        var amountUsed = 0m;

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

            var quote = await _orderBuyerQuoteRepository.Add(request.OrderId, buyerZipCoverageId, amountUsed, now, ct);
            await _uow.SaveChanges(ct);

            newQuoteId = quote.OrderBuyerQuoteId;
            
            await SetCurrentQuote(request.OrderId, newQuoteId, amountUsed, ct);

        }, ct);

        return newQuoteId;
    }

    public async Task SetCurrentQuote(int orderId, int quoteIdCreated, decimal quoteAmountCreated, CancellationToken t = default)
    {
        var quoteIdToUse = quoteIdCreated;
        var quoteSelector = await new QuoteSelector(new MaxAmountQuoteStrategy(_orderBuyerQuoteRepository)).GetBestQuote(orderId);

        if (quoteSelector != null && quoteSelector.Amount > quoteAmountCreated)
            quoteIdToUse = quoteSelector.OrderBuyerQuoteId;

        await _orderRepository.SetCurrentBuyerQuote(orderId, quoteIdToUse, t);
    }
}
