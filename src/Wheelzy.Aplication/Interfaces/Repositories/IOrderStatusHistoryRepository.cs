namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderStatusHistoryRepository
    {
        Task AddAsync(long orderId, int statusId, DateTime statusDateUtc, string changedBy, CancellationToken token);
    }
}
