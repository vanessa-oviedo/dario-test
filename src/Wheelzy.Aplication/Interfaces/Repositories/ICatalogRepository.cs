using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface ICatalogRepository
    {
        Task<Car?> GetCarAsync(int id, CancellationToken ct = default);
    }
}
