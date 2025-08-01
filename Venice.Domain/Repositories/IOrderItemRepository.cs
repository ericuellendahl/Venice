using Venice.Domain.Entities;

namespace Venice.Domain.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
    Task SaveItemsAsync(Guid orderId, List<OrderItem> itens, CancellationToken cancellationToken);
}
