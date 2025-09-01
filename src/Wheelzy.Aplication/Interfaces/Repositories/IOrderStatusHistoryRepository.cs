namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderStatusHistoryRepository
    {
        Task Add(int orderId, int statusId, DateTime statusDateUtc, CancellationToken token);
    }
}
