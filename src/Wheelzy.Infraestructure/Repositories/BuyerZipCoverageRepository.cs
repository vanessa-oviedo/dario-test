using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class BuyerZipCoverageRepository : IBuyerZipCoverageRepository
    {
        private readonly WheetzyDbContext _db;

        public BuyerZipCoverageRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public async Task<(int BuyerZipCoverageId, decimal DefaultQuoteAmount)?> GetCoverage(
            int buyerId, string zipCode, CancellationToken ct)
        {
            var res = await _db.BuyerZipCoverages.AsNoTracking()
                .Where(bzc => bzc.BuyerId == buyerId && bzc.ZipCode == zipCode)
                .Select(bzc => new { bzc.BuyerZipCoverageId, bzc.DefaultQuoteAmount })
                .FirstOrDefaultAsync(ct);

            return res is null ? null : (res.BuyerZipCoverageId, res.DefaultQuoteAmount);
        }
    }
}
