namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteWriteRepository
    {
        Task<int> Add(int orderId, int buyerZipCoverageId, decimal amount, DateTime createdAtUtc, CancellationToken ct);
    }
}
