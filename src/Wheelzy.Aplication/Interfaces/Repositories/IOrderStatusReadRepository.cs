namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderStatusReadRepository
    {
        Task<int?> GetStatusIdByName(string name, CancellationToken ct = default);
    }
}
