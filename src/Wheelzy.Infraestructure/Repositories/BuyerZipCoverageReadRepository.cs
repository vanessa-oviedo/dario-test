using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class BuyerZipCoverageReadRepository : IBuyerZipCoverageReadRepository
    {
        private readonly WheetzyDbContext _db;

        public BuyerZipCoverageReadRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<CoverageProjection>> GetByZipAsync(string zipCode, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                return Array.Empty<CoverageProjection>();

            // Normalizamos por si viene con espacios
            var zip = zipCode.Trim();

            return await _db.BuyerZipCoverages
                .AsNoTracking()
                .Where(bzc => bzc.ZipCode == zip)   // char(5) en SQL compara ok aun con padding
                .Select(bzc => new CoverageProjection(
                    bzc.BuyerZipCoverageId,
                    bzc.DefaultQuoteAmount
                ))
                .ToListAsync(ct);
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
