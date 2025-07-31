using Venice.Domain.ValueObjects;

namespace Venice.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public DateTime Data { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal ValorTotal { get; private set; }

    private Order() { }

    public Order(Guid clienteId, List<OrderItem> itens)
    {
        Id = Guid.NewGuid();
        ClienteId = clienteId;
        Data = DateTime.UtcNow;
        Status = OrderStatus.Criado;

        CalcularValorTotal(itens);
    }

    private void CalcularValorTotal(List<OrderItem> itens)
    => ValorTotal = itens.Sum(item => item.Quantidade * item.PrecoUnitario);

    public void AtualizarStatus(OrderStatus novoStatus)
    => Status = novoStatus;

}
