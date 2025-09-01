using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderStatusReadRepository : IOrderStatusReadRepository
    {
        private readonly WheetzyDbContext _db;

        public OrderStatusReadRepository(WheetzyDbContext db) => _db = db;

        public async Task<int?> GetStatusIdByName(string name, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            return await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name == name)
                .Select(s => (int?)s.StatusId)
                .SingleOrDefaultAsync(ct);
        }
    }
}
