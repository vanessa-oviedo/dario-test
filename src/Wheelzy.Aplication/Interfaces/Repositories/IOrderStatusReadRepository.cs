namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderStatusReadRepository
    {
        Task<int?> GetStatusIdByNameAsync(string name, CancellationToken ct = default);
    }
}
