using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public sealed class CatalogRepository : ICatalogRepository
    {
        private readonly WheetzyDbContext _db;
        public CatalogRepository(WheetzyDbContext db) => _db = db;

        public Task<Make?> GetMakeAsync(int id, CancellationToken ct = default) => _db.Makes.FindAsync(new object?[] { id }, ct).AsTask();
        public Task<Model?> GetModelAsync(int id, CancellationToken ct = default) => _db.Models.FindAsync(new object?[] { id }, ct).AsTask();
        public Task<SubModel?> GetSubModelAsync(int id, CancellationToken ct = default) => _db.SubModels.FindAsync(new object?[] { id }, ct).AsTask();
        public Task<Car?> GetCarAsync(int id, CancellationToken ct = default) => _db.Cars.FindAsync(new object?[] { id }, ct).AsTask();

        public async Task<IReadOnlyList<Make>> ListMakesAsync(CancellationToken ct = default) =>
            await _db.Makes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(ct);

        public async Task<IReadOnlyList<Model>> ListModelsByMakeAsync(int makeId, CancellationToken ct = default) =>
            await _db.Models.AsNoTracking().Where(x => x.MakeId == makeId).OrderBy(x => x.Name).ToListAsync(ct);

        public async Task<IReadOnlyList<SubModel>> ListSubModelsByModelAsync(int modelId, CancellationToken ct = default) =>
            await _db.SubModels.AsNoTracking().Where(x => x.ModelId == modelId).OrderBy(x => x.Name).ToListAsync(ct);
    }
}
