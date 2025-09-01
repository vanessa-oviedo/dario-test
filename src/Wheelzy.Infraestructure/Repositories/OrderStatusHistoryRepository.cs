using System;
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

        public async Task AddAsync(long orderId, int statusId, DateTime statusDateUtc, string changedBy, CancellationToken token)
        {
            var statusName = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.StatusId == statusId)
                .Select(s => s.Name)
                .SingleOrDefaultAsync(token);

            if (statusName is null)
                throw new InvalidOperationException($"OrderStatus not found for id={statusId}");

            // 2) Si es "Picked Up", la fecha es obligatoria (regla de negocio) TODO: COMMENTS IN ENGLISH
            if (string.Equals(statusName, PickedUpName, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"Status '{PickedUpName}' requires a status date.");

            // 3) Insertar historial (la fecha de creación la ponemos acá; tu tabla también tiene default en DB)
            var row = new OrderStatusHistory
            {
                OrderId = orderId,
                StatusId = statusId,
                StatusDate = statusDateUtc,
                ChangedBy = changedBy,
                CreatedAt = DateTime.UtcNow
            };

            await _db.OrderStatusHistories.AddAsync(row, token);
        }
    }
}
