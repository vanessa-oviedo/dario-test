namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteReadRepository
    {
        Task<bool> ExistsForOrderAsync(int orderId, int orderBuyerQuoteId, CancellationToken ct);
    }
}
