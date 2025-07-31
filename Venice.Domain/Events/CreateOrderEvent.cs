using Venice.Domain.Entities;

namespace Venice.Domain.Events;

public sealed record class CreateOrderEvent(Guid PedidoId, Guid ClienteId, decimal ValorTotal, DateTime DataCriacao, List<OrderItem> Itens);

