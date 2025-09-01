using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderBuyerQuoteWriteRepository : IOrderBuyerQuoteWriteRepository
    {
        private readonly WheetzyDbContext _db;

        public OrderBuyerQuoteWriteRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public async Task<int> Add(
            int orderId,
            int buyerZipCoverageId,
            decimal amount,
            DateTime createdAtUtc,
            CancellationToken ct)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be > 0.");

            var entity = new OrderBuyerQuote
            {
                OrderId = orderId,
                BuyerZipCoverageId = buyerZipCoverageId,
                Amount = amount,
                CreatedAt = createdAtUtc
            };

            try
            {
                await _db.OrderBuyerQuotes.AddAsync(entity, ct);
                return entity.BuyerZipCoverageId;
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex, "UQ_Quote_Per_Case_Buyer"))
            {
                throw new InvalidOperationException(
                    $"BuyerZipCoverage {buyerZipCoverageId} already has a quote for Order {orderId}.", ex);
            }
        }

        private static bool IsUniqueViolation(DbUpdateException ex, string constraintName)
        {
            return ex.InnerException is SqlException sqlEx &&
                   (sqlEx.Number == 2627 || sqlEx.Number == 2601) &&
                   (sqlEx.Message?.Contains(constraintName) ?? false);
        }
    }
}
