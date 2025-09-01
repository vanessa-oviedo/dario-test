namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderReadRepository
    {
        Task<string> GetOrderZipAsync(long orderId, CancellationToken ct);
    }
}
