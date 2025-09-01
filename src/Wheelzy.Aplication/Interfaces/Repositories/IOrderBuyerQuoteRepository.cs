using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelzy.Application.Models;

namespace Wheelzy.Application.Interfaces.Repositories
{
    public interface IOrderBuyerQuoteRepository
    {
        Task<IEnumerable<OrderBuyerQuote?>> getByOrderIDandMaxAmmountAsync(int orderID);
    }
}
