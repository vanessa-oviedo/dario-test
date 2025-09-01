using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly WheetzyDbContext _db;
        public OrderRepository(WheetzyDbContext db1) => _db = db1;

        public Task UpdateAsync(Order entity, CancellationToken ct = default)
        {
            _db.Orders.Update(entity);
            return Task.CompletedTask;
        }

        public async Task<int> Add(int customerId, int carId, string zipCode, DateTime now, CancellationToken token)
        {
            var entity = new Order
            {
                CustomerId = customerId,
                CarId = carId,
                ZipCode = zipCode,
                CreatedAt = now
            };
            await _db.Orders.AddAsync(entity, token);   

            return entity.OrderId;
        }

        public async Task SetCurrentStatus(int orderId, int statusId, DateTime? statusDateUtc, CancellationToken token)
        {
            var affected = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(up => up
                        .SetProperty(o => o.CurrentStatusId, statusId)
                        .SetProperty(o => o.CurrentStatusDate, statusDateUtc),
                    token);

            if (affected == 0)
                throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        public async Task<string> GetOrderZip(int orderId, CancellationToken ct)
        {
            var zip = await _db.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => o.ZipCode)   
                .SingleOrDefaultAsync(ct);

            if (zip is null)
                throw new KeyNotFoundException($"Order {orderId} not found.");

            // ZipCode es CHAR(5) en la BD: quitamos padding derecho si lo hubiera
            return zip.TrimEnd();
        }

        public async Task SetCurrentBuyerQuote(int orderId, int orderBuyerQuoteId, string zipCode, CancellationToken ct)
        {
            var affected = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(up =>
                    up.SetProperty(o => o.CurrentOrderBuyerQuoteId, orderBuyerQuoteId), ct);

            if (affected == 0)
                throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        public async Task<IEnumerable<OrderSummaryDto>> SearchSummaries(OrderSearchFilter filter, CancellationToken ct)
        {
            // 0) Base query (solo Orders) + filtros opcionales
            IQueryable<Order> orders = _db.Orders.AsNoTracking();

            if (filter.CreatedFromUtc.HasValue)
                orders = orders.Where(o => o.CreatedAt >= filter.CreatedFromUtc.Value);

            if (filter.CreatedToUtc.HasValue)
            {
                var endExclusive = filter.CreatedToUtc.Value.Date.AddDays(1);
                orders = orders.Where(o => o.CreatedAt < endExclusive);
            }

            if (filter.CustomerIds?.Count() > 0) // <-- corregido
                orders = orders.Where(o => filter.CustomerIds.Contains(o.CustomerId));

            if (filter.Statuses?.Count() > 0)
                orders = orders.Where(o => o.CurrentStatusId.HasValue && filter.Statuses.Contains(o.CurrentStatusId.Value));

            if (filter.IsActive.HasValue)
            {
                var pickedUpId = await _db.OrderStatuses.AsNoTracking()
                    .Where(s => s.Name == "Picked Up")
                    .Select(s => (int?)s.StatusId)
                    .SingleOrDefaultAsync(ct);

                if (pickedUpId.HasValue)
                {
                    orders = filter.IsActive.Value
                        ? orders.Where(o => !o.CurrentStatusId.HasValue || o.CurrentStatusId != pickedUpId.Value)
                        : orders.Where(o => o.CurrentStatusId.HasValue && o.CurrentStatusId == pickedUpId.Value);
                }
            }

            // 1) JOINs necesarios (auto)
            var query =
                from o in orders
                join car in _db.Cars.AsNoTracking() on o.CarId equals car.CarId
                join sm in _db.CarSubmodels.AsNoTracking() on car.SubmodelId equals sm.SubmodelId
                join md in _db.CarModels.AsNoTracking() on sm.ModelId equals md.ModelId
                join mk in _db.CarMakes.AsNoTracking() on md.MakeId equals mk.MakeId

                select new
                {
                    o,
                    car,
                    sm,
                    md,
                    mk
                };

            // 2) Traer datos a memoria para los DTOs (solo la parte que EF Core no puede traducir)
            var list = query
                .AsEnumerable() // <-- EF deja de traducir a SQL, ahora LINQ normal
                .Select(x =>
                {
                    // status actual
                    var currentStatusName = _db.OrderStatuses
                        .AsNoTracking()
                        .FirstOrDefault(s => s.StatusId == x.o.CurrentStatusId)?.Name;

                    // quote actual
                    var cq = (from q in _db.OrderBuyerQuotes.AsNoTracking()
                              join z in _db.BuyerZipCoverages.AsNoTracking() on q.BuyerZipCoverageId equals z.BuyerZipCoverageId
                              join b in _db.Buyers.AsNoTracking() on z.BuyerId equals b.BuyerId
                              where q.OrderId == x.o.OrderId
                                 && q.OrderBuyerQuoteId == x.o.CurrentOrderBuyerQuoteId
                              select new { q.Amount, BuyerName = b.Name })
                              .FirstOrDefault();

                    return new OrderSummaryDto(
                        x.o.OrderId,
                        x.o.CreatedAt,
                        x.car.Year,
                        x.mk.Name,
                        x.md.Name,
                        x.sm.Name,
                        cq?.BuyerName,
                        cq?.Amount,
                        currentStatusName,
                        x.o.CurrentStatusDate
                    );
                })
                .OrderByDescending(d => d.CreatedAtUtc)
                .ThenBy(d => d.OrderId)
                .ToList();

            return list;
        }

        public async Task<bool> Exists(int orderId)
        {
            return await _db.Orders.AnyAsync(a => a.OrderId == orderId);
        }
    }
}
