using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public sealed class BuyerRepository : IBuyerRepository
    {
        private readonly WheetzyDbContext _db;
        public BuyerRepository(WheetzyDbContext db) => _db = db;

        public Task<Buyer?> GetByIdAsync(int buyerId, CancellationToken ct = default) =>
            _db.Buyers.FindAsync(new object?[] { buyerId }, ct).AsTask();

        public async Task<IReadOnlyList<Buyer>> GetAllAsync(CancellationToken ct = default) =>
            await _db.Buyers.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);
    }
}
