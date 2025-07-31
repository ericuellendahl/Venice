using Venice.Domain.Entities;

namespace Venice.Domain.Repositories;

public interface IOrderItemRepository
{
    Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId);
    Task SaveItemsAsync(Guid orderId, List<OrderItem> itens);
}
