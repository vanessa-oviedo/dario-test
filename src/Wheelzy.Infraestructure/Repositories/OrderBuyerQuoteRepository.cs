using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<OrderBuyerQuote?> getByOrderIDandMaxAmmountAsync(int orderID)
        {
            var orderBuyerQuoteResult =await _db.OrderBuyerQuotes
                .Where(obq => obq.OrderId == orderID)
                .OrderByDescending(obq => obq.Amount).FirstOrDefaultAsync();
            
            return orderBuyerQuoteResult;
        }
    }
}
