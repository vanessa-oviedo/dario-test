using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Domain;

namespace Wheelzy.Infrastructure;

public interface ICachedLookups
{
    Task<List<Status>> GetStatusesAsync(CancellationToken ct);
    Task<List<Make>> GetMakesAsync(CancellationToken ct);
}

public class CachedLookups(AppDbContext db, IMemoryCache cache) : ICachedLookups
{
    private static readonly MemoryCacheEntryOptions Opt = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(10),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public Task<List<Status>> GetStatusesAsync(CancellationToken ct) =>
        cache.GetOrCreateAsync("lkp:statuses", async entry =>
        {
            entry.SetOptions(Opt);
            return await db.Statuses.AsNoTracking().OrderBy(s => s.StatusId).ToListAsync(ct);
        })!;

    public Task<List<Make>> GetMakesAsync(CancellationToken ct) =>
        cache.GetOrCreateAsync("lkp:makes", async entry =>
        {
            entry.SetOptions(Opt);
            return await db.Makes.Include(m => m.Models).ThenInclude(m => m.Submodels)
                                 .AsNoTracking().ToListAsync(ct);
        })!;
}
