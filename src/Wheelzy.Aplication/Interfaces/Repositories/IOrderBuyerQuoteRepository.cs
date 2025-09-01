using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteRepository
    {
        Task<OrderBuyerQuote?> GetByOrderIDandMaxAmount(int orderID);

        Task<OrderBuyerQuote> Add(
            int orderId,
            int buyerZipCoverageId,
            decimal amount,
            DateTime createdAtUtc,
            CancellationToken ct);
    }
}
