using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteRepository
    {
        Task<IEnumerable<OrderBuyerQuote?>> GetByOrderIDandMaxAmmountAsync(int orderID);

        Task<int> Add(
            int orderId,
            int buyerZipCoverageId,
            decimal amount,
            DateTime createdAtUtc,
            CancellationToken ct);
    }
}
