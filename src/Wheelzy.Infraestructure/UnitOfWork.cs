using System.Data;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces;

namespace Wheelzy.Infrastructure
{
    //TODO: REMOVE COMMENTS IN SPANISH
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly Persistence.WheetzyDbContext _db;
        public UnitOfWork(Persistence.WheetzyDbContext db) => _db = db;

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public void Dispose() => _db.Dispose();

        public async Task ExecuteInTransactionAsync(Func<object, Task> action, CancellationToken ct)
        {
            // Usa la estrategia de ejecución del proveedor (SQL Server: reintentos ante errores transitorios)
            var strategy = _db.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                // Si ya existe una transacción (p. ej. llamada anidada), usá savepoint si está disponible;
                // si no, simplemente ejecutá la acción dentro de la transacción existente.
                var currentTx = _db.Database.CurrentTransaction;
                if (currentTx is not null)
                {
                    // EF Core 7/8: savepoints (SQL Server los soporta)
                    var savepoint = "sp_" + Guid.NewGuid().ToString("N");
                    try
                    {
                        await currentTx.CreateSavepointAsync(savepoint, ct);
                        await action(ct);
                    }
                    catch
                    {
                        // revertir solo lo hecho por esta acción
                        await currentTx.RollbackToSavepointAsync(savepoint, ct);
                        throw;
                    }
                    return;
                }

                // No había transacción: creamos una y controlamos commit/rollback
                await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
                try
                {
                    await action(ct);          // tus services llaman SaveChangesAsync cuando corresponda
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
