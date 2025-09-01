using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
    {
        private readonly WheetzyDbContext _db;
        private const string PickedUpName = "Picked Up"; //TODO: MOVE TO A CONSTANTS CLASS

        public OrderStatusHistoryRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public async Task Add(int orderId, int statusId, DateTime statusDateUtc, CancellationToken token)
        {
            var statusName = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.StatusId == statusId)
                .Select(s => s.Name)
                .SingleOrDefaultAsync(token);

            if (statusName is null)
                throw new InvalidOperationException($"OrderStatus not found for id={statusId}");

            if (string.Equals(statusName, PickedUpName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Status '{PickedUpName}' requires a status date.");

            var row = new OrderStatusHistory
            {
                OrderId = orderId,
                StatusId = statusId,
                StatusDate = statusDateUtc,
                CreatedAt = DateTime.UtcNow
            };

            await _db.OrderStatusHistories.AddAsync(row, token);
        }
    }
}
