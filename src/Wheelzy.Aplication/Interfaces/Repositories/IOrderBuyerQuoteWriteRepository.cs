namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteWriteRepository
    {
        Task<int> AddAsync(long orderId, int buyerZipCoverageId, decimal amount, DateTime createdAtUtc, CancellationToken ct);
    }
}
