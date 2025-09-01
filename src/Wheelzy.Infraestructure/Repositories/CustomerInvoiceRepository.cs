using Microsoft.EntityFrameworkCore;
using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Models;
using Wheelzy.Infrastructure.Persistence;

namespace Wheelzy.Infrastructure.Repositories
{
    public class CustomerInvoiceRepository : ICustomerInvoiceRepository
    {
        private readonly WheetzyDbContext _db;

        public CustomerInvoiceRepository(WheetzyDbContext db)
        {
            _db = db;
        }

        public async Task UpdateCustomersBalanceByInvoices(List<Invoice> invoices, CancellationToken token)
        {
            if (invoices is null || invoices.Count == 0) return;

            await using var tx = await _db.Database.BeginTransactionAsync(token);

            _db.Invoices.AddRange(invoices);
            await _db.SaveChangesAsync(token);

            var deltas = invoices
                .GroupBy(i => i.CustomerId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(i => i.Amount)
                );

            foreach (var (customerId, delta) in deltas)
            {
                var affected = await _db.Customers
                    .Where(c => c.CustomerId == customerId)
                    .ExecuteUpdateAsync(up =>
                        up.SetProperty(c => c.Balance, c => c.Balance - delta),
                        token);

                if (affected == 0)
                    throw new InvalidOperationException($"Customer {customerId} not found.");
            }

            await tx.CommitAsync(token);
        }
    }
}
