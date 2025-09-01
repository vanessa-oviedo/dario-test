using Wheelzy.Application.Interfaces.Strategy;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Strategy
{

    public class QuoteSelector
    {
        private IQuoteSelectionStrategy _quoteSelectionStrategy;

        public QuoteSelector(IQuoteSelectionStrategy quoteSelectionStrategy)
        {
            _quoteSelectionStrategy = quoteSelectionStrategy;
        }

        public void SetStrategy(IQuoteSelectionStrategy strategy)
        {
            _quoteSelectionStrategy = strategy;
        }

        public async Task<OrderBuyerQuote?> GetBestQuote(int orderId)
        {
            return await _quoteSelectionStrategy.Select(orderId);
        }

    }
}
