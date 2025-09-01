namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteReadRepository
    {
        Task<bool> ExistsForOrderAsync(long orderId, long value, CancellationToken ct);
    }
}
