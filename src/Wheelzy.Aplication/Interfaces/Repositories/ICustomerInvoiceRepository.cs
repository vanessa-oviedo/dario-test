using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface ICustomerInvoiceRepository
    {
        Task UpdateCustomersBalanceByInvoices(List<Invoice> invoices, CancellationToken token);
    }
}
