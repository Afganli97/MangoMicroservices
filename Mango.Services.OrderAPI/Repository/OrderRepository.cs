using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<AppDbContext> _dbContext;

        public OrderRepository(DbContextOptions<AppDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHeader)
        {
            await using var _context = new AppDbContext(_dbContext);
            _context.OrderHeaders.Add(orderHeader);
            await _context.SaveChangesAsync();
            return true; 
        }

        public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
        {
            await using var _context = new AppDbContext(_dbContext);
            var orderHeaderFromDb = await _context.OrderHeaders.FindAsync(orderHeaderId);
            if (orderHeaderFromDb != null)
            {
                orderHeaderFromDb.PaymentStatus = paid;
                await _context.SaveChangesAsync();
            }
        }
    }
}