using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderReadRepository : IOrderReadRepository
    {
        private readonly WheetzyDbContext _db;
        public OrderReadRepository(WheetzyDbContext db) => _db = db;

        public async Task<string> GetOrderZipAsync(int orderId, CancellationToken ct)
        {
            var zip = await _db.Orders.AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => o.ZipCode)
                .SingleOrDefaultAsync(ct);

            if (zip is null) throw new KeyNotFoundException(); //TODO: Handle errors
            return zip.TrimEnd();
        }
    }
}
