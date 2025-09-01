using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class OrderBuyerQuoteRepository : IOrderBuyerQuoteRepository
    {
        private readonly WheetzyDbContext _db;

        public OrderBuyerQuoteRepository(WheetzyDbContext db) => _db = db;
       
        public async Task<OrderBuyerQuote?> GetByOrderIDandMaxAmmountAsync(int orderID)
        {
            var orderBuyerQuoteResult =await _db.OrderBuyerQuotes
                .Where(obq => obq.OrderId == orderID)
                .OrderByDescending(obq => obq.Amount).FirstOrDefaultAsync();
            
            return orderBuyerQuoteResult;
        }

        public async Task<OrderBuyerQuote> Add(
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
                var entry = await _db.OrderBuyerQuotes.AddAsync(entity, ct);
                return entry.Entity;    
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
