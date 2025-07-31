using Venice.Domain.Entities;

namespace Venice.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order> CreateAsync(Order order);
    Task UpdateAsync(Order order);
}
