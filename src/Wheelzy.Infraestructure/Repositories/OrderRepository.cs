using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Wheelzy.Application.Interfaces.Queries;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<List<OrderCurrentSummaryDto>> GetCurrentSummariesAsync(CancellationToken ct = default)
        {
            var query =
                from o in _db.Orders.AsNoTracking()
                join car in _db.Cars.AsNoTracking() on o.CarId equals car.CarId
                join sm in _db.CarSubmodels.AsNoTracking() on car.SubmodelId equals sm.SubmodelId
                join md in _db.CarModels.AsNoTracking() on sm.ModelId equals md.ModelId
                join mk in _db.CarMakes.AsNoTracking() on md.MakeId equals mk.MakeId

                // LEFT “current quote” (compose: OrderId + CurrentOrderBuyerQuoteId)
                let currentQuote = (
                    from q in _db.OrderBuyerQuotes.AsNoTracking()
                    join z in _db.BuyerZipCoverages.AsNoTracking() on q.BuyerZipCoverageId equals z.BuyerZipCoverageId
                    join b in _db.Buyers.AsNoTracking() on z.BuyerId equals b.BuyerId
                    where q.OrderId == o.OrderId
                          && (long?)q.OrderBuyerQuoteId == o.CurrentOrderBuyerQuoteId
                    select new { q.Amount, BuyerName = b.Name }
                ).FirstOrDefault()

                // LEFT status actual (CurrentStatusId es int? en Order)
                let currentStatusName = (
                    from s in _db.OrderStatuses.AsNoTracking()
                    where (int?)s.StatusId == o.CurrentStatusId
                    select s.Name
                ).FirstOrDefault()

                select new OrderCurrentSummaryDto(
                    OrderId: o.OrderId,
                    CarYear: car.Year,
                    Make: mk.Name,
                    Model: md.Name,
                    SubModel: sm.Name,
                    CurrentBuyerName: currentQuote == null ? null : currentQuote.BuyerName,
                    CurrentQuoteAmount: currentQuote == null ? null : (decimal?)currentQuote.Amount,
                    CurrentStatusName: currentStatusName,
                    CurrentStatusDate: o.CurrentStatusDate
                );

            return await query
                .OrderBy(x => x.OrderId)   // opcional
                .ToListAsync(ct);
        }

        Task<Order?> IOrderRepository.GetByIdAsync(int caseId, bool includeRelated, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(int customerId, int carId, string zipCode, DateTime now, CancellationToken token)
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

        public Task<CaseQuote?> GetCurrentQuoteAsync(int caseId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<CaseStatusHistory?> GetCurrentStatusAsync(int caseId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(int caseId, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task SetCurrentStatusAsync(long orderId, int statusId, DateTime? statusDateUtc, string changedBy, CancellationToken ct)
        {
            string? by = string.IsNullOrWhiteSpace(changedBy)
                ? null
                : (changedBy.Length > 200 ? changedBy[..200] : changedBy);

            var affected = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(up => up
                        .SetProperty(o => o.CurrentStatusId, statusId)
                        .SetProperty(o => o.CurrentStatusDate, statusDateUtc)
                        .SetProperty(o => o.CurrentStatusChangedBy, by),
                    ct);

            if (affected == 0)
                throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        public async Task<string> GetOrderZipAsync(long orderId, CancellationToken ct)
        {
            var zip = await _db.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == orderId)
                .Select(o => o.ZipCode)           // mapea a string
                .SingleOrDefaultAsync(ct);

            if (zip is null)
                throw new KeyNotFoundException($"Order {orderId} not found.");

            // ZipCode es CHAR(5) en la BD: quitamos padding derecho si lo hubiera
            return zip.TrimEnd();
        }

        public async Task SetCurrentBuyerQuoteAsync(long orderId, long? orderBuyerQuoteId, CancellationToken ct)
        {
            var affected = await _db.Orders
                .Where(o => o.OrderId == orderId)
                .ExecuteUpdateAsync(up =>
                    up.SetProperty(o => o.CurrentOrderBuyerQuoteId, orderBuyerQuoteId), ct);

            if (affected == 0)
                throw new KeyNotFoundException($"Order {orderId} not found.");
        }

        public async Task<IEnumerable<SellCaseSummaryDto>> SearchSummariesAsync(OrderSearchFilter filter, CancellationToken ct)
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

            if (filter.CustomerIds.Count() == 0)
                orders = orders.Where(o => filter.CustomerIds.Contains(o.CustomerId));

            if (filter.Statuses.Count() > 0)
                orders = orders.Where(o => o.CurrentStatusId.HasValue && filter.Statuses.Contains(o.CurrentStatusId.Value));

            if (filter.IsActive.HasValue)
            {
                // Consideramos “inactivo” = Picked Up (según el requerimiento)
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
                // si no existe "Picked Up" en catálogo, no aplicamos este filtro
            }

            // 1) JOINs necesarios (auto) + LEFT APPLY (status y quote actual)
            var query =
                from o in orders
                join car in _db.Cars.AsNoTracking() on o.CarId equals car.CarId
                join sm in _db.CarSubmodels.AsNoTracking() on car.SubmodelId equals sm.SubmodelId
                join md in _db.CarModels.AsNoTracking() on sm.ModelId equals md.ModelId
                join mk in _db.CarMakes.AsNoTracking() on md.MakeId equals mk.MakeId

                // LEFT status actual (por Id nullable)
                let currentStatusName =
                    (from s in _db.OrderStatuses.AsNoTracking()
                     where (int?)s.StatusId == o.CurrentStatusId
                     select s.Name).FirstOrDefault()

                // LEFT current quote (compuesto: OrderId + OrderBuyerQuoteId)
                let cq =
                    (from q in _db.OrderBuyerQuotes.AsNoTracking()
                     join z in _db.BuyerZipCoverages.AsNoTracking() on q.BuyerZipCoverageId equals z.BuyerZipCoverageId
                     join b in _db.Buyers.AsNoTracking() on z.BuyerId equals b.BuyerId
                     where q.OrderId == o.OrderId
                        && (long?)q.OrderBuyerQuoteId == o.CurrentOrderBuyerQuoteId
                     select new { q.Amount, BuyerName = b.Name })
                    .FirstOrDefault()
                let sellCaseSummaryDto = new SellCaseSummaryDto(
                                    OrderId: o.OrderId,
                                    CreatedAtUtc: o.CreatedAt,
                                    CarYear: car.Year,
                                    Make: mk.Name,
                                    Model: md.Name,
                                    SubModel: sm.Name,
                                    CurrentBuyerName: cq == null ? null : cq.BuyerName,
                                    CurrentQuoteAmount: cq == null ? null : (decimal?)cq.Amount,
                                    CurrentStatusName: currentStatusName,
                                    CurrentStatusDateUtc: o.CurrentStatusDate
                                )
                select sellCaseSummaryDto;

            // 2) Ordenar (opcional) y ejecutar
            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenBy(x => x.OrderId)
                .ToListAsync(ct);
        }
    }
}
