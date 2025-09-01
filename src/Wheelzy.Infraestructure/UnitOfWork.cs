using System.Data;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces;

namespace Wheelzy.Infrastructure
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Persistence.WheetzyDbContext _db;
        public UnitOfWork(Persistence.WheetzyDbContext db) => _db = db;

        public Task<int> SaveChanges(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public void Dispose() => _db.Dispose();

        public async Task ExecuteInTransactionAsync(Func<object, Task> action, CancellationToken ct)
        {
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                var currentTx = _db.Database.CurrentTransaction;
                if (currentTx is not null)
                {
                    var savepoint = "sp_" + Guid.NewGuid().ToString("N");
                    try
                    {
                        await currentTx.CreateSavepointAsync(savepoint, ct);
                        await action(ct);
                    }
                    catch
                    {
                        await currentTx.RollbackToSavepointAsync(savepoint, ct);
                        throw;
                    }
                    return;
                }

                await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
                try
                {
                    await action(ct);      
                    await tx.CommitAsync(ct);
                }
                catch
                {
                    await tx.RollbackAsync(ct);
                    throw;
                }
            });
        }
    }
}
