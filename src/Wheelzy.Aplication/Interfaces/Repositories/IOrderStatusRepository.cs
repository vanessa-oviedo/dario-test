namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderStatusRepository
    {
        Task<int?> GetStatusIdByName(string name, CancellationToken ct = default);
    }
}
