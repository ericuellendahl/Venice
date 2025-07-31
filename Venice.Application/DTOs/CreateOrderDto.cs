using System.ComponentModel.DataAnnotations;

namespace Venice.Application.DTOs;

public class CreateOrderDto
{
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public Guid ClienteId { get; set; }
    public List<OrderItemDto> Itens { get; set; } = new();
}
