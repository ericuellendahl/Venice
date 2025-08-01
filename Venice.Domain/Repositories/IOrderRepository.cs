using Venice.Domain.Entities;

namespace Venice.Domain.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken);
    Task UpdateAsync(Order order, CancellationToken cancellationToken);
    Task RemoveAsync(Order order, CancellationToken cancellationToken);
}
