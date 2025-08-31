using Wheelzy.Application.Interfaces;

namespace Wheelzy.Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly Persistence.WheetzyDbContext _db;
        public UnitOfWork(Persistence.WheetzyDbContext db) => _db = db;
        public Task<int> SaveChangesAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
    }
}
