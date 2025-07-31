namespace Venice.Application.DTOs;

public class OrderItemDto
{
    public string Produto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
