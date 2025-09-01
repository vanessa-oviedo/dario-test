namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderReadRepository
    {
        Task<string> GetOrderZipAsync(int orderId, CancellationToken ct);
    }
}
