using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Strategy
{
    public interface IQuoteSelectionStrategy
    {
        Task<OrderBuyerQuote?> Select(int orderId);
    }
}
