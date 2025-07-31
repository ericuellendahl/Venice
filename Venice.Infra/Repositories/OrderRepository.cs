using Microsoft.EntityFrameworkCore;
using Venice.Domain.Entities;
using Venice.Domain.Repositories;
using Venice.Infra.Data;

namespace Venice.Infra.Repositories
{
    public class OrderRepository(VeniceDbContext context) : IOrderRepository
    {
        private readonly VeniceDbContext _context = context;

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> CreateAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
