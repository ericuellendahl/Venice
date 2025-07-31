using System.ComponentModel.DataAnnotations;

namespace Venice.Application.DTOs;

public class OrderItemDto
{
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public string Produto { get; set; }
    
    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public int Quantidade { get; set; }

    [Required(ErrorMessage = "O {0} é obrigatório.")]
    public decimal PrecoUnitario { get; set; }
}
