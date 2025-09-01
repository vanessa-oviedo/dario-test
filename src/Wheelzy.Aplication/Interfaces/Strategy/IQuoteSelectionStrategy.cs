using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Strategy
{
    public interface IQuoteSelectionStrategy
    {
        Task<IEnumerable<OrderBuyerQuote?>> Select(int orderId);
    }
}
