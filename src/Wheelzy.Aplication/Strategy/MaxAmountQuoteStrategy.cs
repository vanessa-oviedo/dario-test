using Wheelzy.Application.Interfaces.Repositories;
using Wheelzy.Application.Interfaces.Strategy;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Strategy
{
    public class MaxAmountQuoteStrategy(IOrderBuyerQuoteRepository orderBuyerQuoteRepository) : IQuoteSelectionStrategy
    {
        private readonly IOrderBuyerQuoteRepository _orderBuyerQuoteRepository = orderBuyerQuoteRepository;

        public async Task<IEnumerable<OrderBuyerQuote?>> Select(int orderId)
        {
            return await _orderBuyerQuoteRepository.getByOrderIDandMaxAmmountAsync(orderId);
        }
    }
}
