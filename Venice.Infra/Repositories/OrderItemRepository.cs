using MongoDB.Driver;
using Venice.Domain.Entities;
using Venice.Domain.Repositories;
using Venice.Infra.Data;
using Venice.Infra.Models;

namespace Venice.Infra.Repositories
{
    public class OrderItemRepository(MongoDbContext _context) : IOrderItemRepository
    {
        public async Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken)
        {
            var documents = await _context.OrderItems
                .Find(x => x.OrderId == orderId)
                .ToListAsync(cancellationToken);

            return [.. documents.Select(d => d.ToDomain())];
        }

        public async Task SaveItemsAsync(Guid orderId, List<OrderItem> itens, CancellationToken cancellationToken)
        {
            var documents = itens.Select(item => OrderItemDocument.FromDomain(orderId, item));
            await _context.OrderItems.InsertManyAsync(documents, null, cancellationToken);
        }
    }
}
