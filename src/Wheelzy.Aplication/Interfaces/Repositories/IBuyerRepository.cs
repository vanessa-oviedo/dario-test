using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IBuyerRepository
    {
        Task<Buyer?> GetByIdAsync(int buyerId, CancellationToken ct = default);
        Task<IReadOnlyList<Buyer>> GetAllAsync(CancellationToken ct = default);
    }
}
