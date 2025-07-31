namespace Venice.Application.DTOs;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime Data { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public List<OrderItemDto> Itens { get; set; } = new();
}
