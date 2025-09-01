using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderBuyerQuoteReadRepository : IOrderBuyerQuoteReadRepository
    {
        private readonly WheetzyDbContext _db;

        public OrderBuyerQuoteReadRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public Task<bool> ExistsForOrderAsync(int orderId, int orderBuyerQuoteId, CancellationToken ct = default)
            => _db.OrderBuyerQuotes
                .AsNoTracking()
                .AnyAsync(q => q.OrderId == orderId && q.OrderBuyerQuoteId == orderBuyerQuoteId, ct);
    }
}

