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

        public Task<Car?> GetCarAsync(int id, CancellationToken ct = default) => _db.Cars.FindAsync(new object?[] { id }, ct).AsTask();
    }
}
