using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public sealed class RateRepository : IRateRepository
    {
        private readonly WheetzyDbContext _db;
        public RateRepository(WheetzyDbContext db) => _db = db;

        public async Task<IReadOnlyList<(int BuyerId, decimal Amount)>> GetBaseRatesByZipAsync(string zipCode, CancellationToken ct = default)
        {
            var rows = await _db.BuyerZipRates
                .AsNoTracking()
                .Where(x => x.ZipCode == zipCode)
                .Select(x => new { x.BuyerId, x.Amount })
                .ToListAsync(ct);

            return rows.Select(r => (r.BuyerId, r.Amount)).ToList();
        }
    }
}
