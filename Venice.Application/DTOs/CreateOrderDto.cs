namespace Venice.Application.DTOs;

public class CreateOrderDto
{
    public Guid ClienteId { get; set; }
    public List<OrderItemDto> Itens { get; set; } = new();
}
